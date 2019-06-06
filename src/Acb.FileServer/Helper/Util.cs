using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.FileServer.Domain;
using Acb.FileServer.Enums;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Acb.FileServer.Helper
{
    internal static class Util
    {
        /// <summary> 判断是否缓存在浏览器中 </summary>
        /// <param name="context"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool IsCachedOnBrowser(HttpContext context, string hash)
        {
            return false;
            //if (!string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_IF_NONE_MATCH"]) &&
            //    context.Request.ServerVariables["HTTP_IF_NONE_MATCH"].Equals(hash))
            //{
            //    context.Response.Headers.Clear();
            //    context.Response.StatusCode = 304;
            //    context.Response.Headers.Add("Content-Length", "0");
            //    return true;
            //}
            //return false;
        }

        /// <summary>
        /// 获取本地文件内容
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="context"></param>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        public static string GetLocalFile(Uri uri, HttpContext context, List<string> fileNames)
        {

            string html;
            try
            {
                var env = context.RequestServices.GetService<IHostingEnvironment>();
                var path = Path.Combine(env.WebRootPath, uri.AbsolutePath);
                html = File.ReadAllText(path);
                fileNames.Add(path);
            }
            catch
            {
                html = GetRemoteFile(uri);
            }
            return html;
        }

        /// <summary> 获取远程文件 </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static string GetRemoteFile(Uri uri)
        {
            var html = new StringBuilder();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                using (var resp = request.GetResponse() as HttpWebResponse)
                {
                    if (resp == null) return string.Empty;
                    using (var recDataStream = resp.GetResponseStream())
                    {
                        if (recDataStream == null) return string.Empty;
                        var buffer = new byte[1024];
                        int read;
                        do
                        {
                            read = recDataStream.Read(buffer, 0, buffer.Length);
                            html.Append(Encoding.UTF8.GetString(buffer, 0, read));
                        }
                        while (read > 0);
                    }
                }
            }
            catch (Exception)
            {
                return html.ToString();
            }
            return html.ToString();
        }

        /// <summary>
        /// 获取新文件名
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string GenerateFileName(string ext = null)
        {
            var name = IdentityHelper.Guid32;
            return string.Concat(name, ext ?? string.Empty);
        }

        public static void ResponseJson(string str, HttpContext context = null)
        {
            context = context ?? AcbHttpContext.Current;
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.WriteAsync(str);
        }

        /// <summary> 返回图片格式 </summary>
        /// <param name="context"></param>
        /// <param name="defaultFile"></param>
        internal static async Task ResponseImage(HttpContext context = null, string defaultFile = null)
        {
            context = context ?? AcbHttpContext.Current;
            if (context == null)
                return;
            var path = string.IsNullOrWhiteSpace(defaultFile) ? context.Request.Path.Value : defaultFile;
            var ext = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(ext))
                return;
            //var reg = new Regex(Constants.ImageUrlRegex, RegexOptions.IgnoreCase);
            var uri = context.Request.GetAbsoluteUri();
            var hash = uri.Md5();
            if (IsCachedOnBrowser(context, hash))
                return;

            //int width = -1, height = -1;
            var (filename, width, height) = uri.ImageSize();
            if (width == 0) width = -1;
            if (height == 0) height = -2;
            path = filename;
            //if (reg.IsMatch(uri))
            //{
            //    var m = reg.Match(uri);
            //    width = ConvertHelper.StrToInt(m.Groups["w"].Value, -1);
            //    height = ConvertHelper.StrToInt(m.Groups["h"].Value, -2);
            //    if (width == -1 && height < -1) height = -1;
            //    path = path?.Replace(m.Groups[1].Value, string.Empty);
            //}
            var resp = context.Response;
            resp.Headers.Add("Vary", "Accept-Encoding");
            resp.Headers.Add("Cache-Control", "max-age=604800");
            resp.Headers.Add("Expires", DateTime.Now.AddYears(1).ToString("R"));
            resp.Headers.Add("ETag", hash);
            resp.ContentType = Constants.MiniType[ext];
            resp.Headers.Add("Charset", "utf-8");
            var fileStream = Stream.Null;
            if (path != null && path.StartsWith("/gfs/"))
            {
                var name = Path.GetFileNameWithoutExtension(path);
                var db = path.Split('/')[2];
                fileStream = await new GridFsManager(db).Read(name);
                if (fileStream == Stream.Null)
                    path = context.MapPath(Constants.DefaultImage);
            }
            else
            {
                path = context.MapPath(path);
            }

            using (var img = fileStream == Stream.Null ? new ImageCls(path) : new ImageCls(fileStream))
            {
                var bit = img.ResizeImg(width, height);
                var encoderParameters =
                    new EncoderParameters(1)
                    {
                        Param = { [0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 88L) }
                    };
                var encoder = GetEncoderInfo(ext);
                bit.Save(resp.Body, encoder, encoderParameters);
            }
        }

        /// <summary> 解压文件 </summary>
        /// <param name="stream">文件流</param>
        /// <param name="savePath">保存路径</param>
        public static async Task UnZipFile(Stream stream, string savePath)
        {
            using (var s = new ZipInputStream(stream))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    var fileName = Path.GetFileName(theEntry.Name);
                    if (string.IsNullOrWhiteSpace(fileName))
                        continue;
                    // create directory
                    var directoryName = Path.GetDirectoryName(theEntry.Name);
                    if (!string.IsNullOrWhiteSpace(directoryName))
                        savePath = Path.Combine(savePath, directoryName);
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);
                    var filePath = Path.Combine(savePath, fileName);
                    using (var streamWriter = File.Create(filePath))
                    {
                        var size = 2048;
                        var data = new byte[size];
                        while ((size = await s.ReadAsync(data, 0, data.Length)) > 0)
                        {
                            await streamWriter.WriteAsync(data, 0, size);
                        }
                    }
                }
            }
        }

        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            //根据 mime 类型，返回编码器
            var encoders = ImageCodecInfo.GetImageEncoders();
            mimeType = "image/" + mimeType.TrimStart('.').ToLower();
            mimeType = mimeType.Replace("jpg", "jpeg");
            return encoders.FirstOrDefault(t => t.MimeType == mimeType);
        }
    }
}
