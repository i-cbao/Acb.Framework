using Acb.Core;
using Acb.FileServer.Enums;
using Acb.FileServer.Helper;
using Acb.FileServer.ViewModels;
using Acb.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Acb.FileServer.Controllers
{
    public class HomeController : DController
    {
        private readonly DirectoryHelper _directoryHelper;

        public HomeController(DirectoryHelper directoryHelper)
        {
            _directoryHelper = directoryHelper;
        }

        /// <summary> 上传文件 </summary>
        /// <param name="type"></param>
        /// <param name="mongo"></param>
        /// <returns></returns>
        [HttpPost("uploader")]
        public async Task<DResult> Upload(FileType type = FileType.Image, bool mongo = false)
        {
            var input = FromBody<VUploadInput>();
            var helper = new UploadHelper(_directoryHelper, type);
            IFormFileCollection files = null;
            var result = new Dictionary<string, string>();
            if (HttpContext.Request.HasFormContentType)
            {
                //Form文件处理
                files = Request.Form.Files;
            }
            else if (input != null && input.Base64.IsBase64())
            {
                //过滤特殊字符
                var b64 = input.Base64;
                var arr = b64.Split(',');
                if (arr.Length == 2)
                    b64 = arr[1];
                b64 = b64.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+").Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                if (b64.Length % 4 > 0)
                    b64 = b64.PadRight(b64.Length + 4 - b64.Length % 4, '=');
                var stream = new MemoryStream(Convert.FromBase64String(b64));
                files = new FormFileCollection
                {
                    new FormFile(stream, 0, stream.Length, input.Name, input.FileName)
                };
            }

            if (files != null)
            {
                helper.CheckFormFile(files);
                result = await (mongo ? helper.GridFsSave(files) : helper.FileSave(files));
            }

            if (result.Count == 0)
                return Error("请上传文件");
            return Succ(result);
        }

        /// <summary> mongo文件展示 </summary>
        /// <param name="database"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpGet("gfs/{database}/{file}"), ResponseCache(Duration = 604800)]
        public async Task Mongo(string database, string file)
        {
            var manager = new GridFsManager(database);
            var ext = Path.GetExtension(file);
            string name;
            int width, height;
            if (ext.IsImage())
            {
                (name, width, height) = file.ImageSize();
            }
            else
            {
                name = file;
                width = 0;
                height = 0;
            }

            name = Path.GetFileNameWithoutExtension(name);
            var gridFs = await manager.Read(name);
            if (!gridFs.FileInfo.Metadata.TryGetValue("ContentType", out var contentType))
            {
                contentType = Constants.MiniType.TryGetValue(ext, out var miniType) ? miniType : "application/octet-stream";
            }

            var resp = HttpContext.Response;
            resp.StatusCode = StatusCodes.Status200OK;
            resp.ContentType = contentType.AsString;

            if (width != 0 || height != 0)
            {
                if (width == 0) width = -1;
                if (height == 0) height = -2;
                if (width == -1 && height < -1) height = -1;
                using (var cls = new ImageCls(gridFs))
                {
                    var bmp = cls.ResizeImg(width, height);
                    var encoderParameters =
                        new EncoderParameters(1)
                        {
                            Param = { [0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 88L) }
                        };
                    var encoder = Util.GetEncoderInfo(ext);
                    bmp.Save(resp.Body, encoder, encoderParameters);
                }
            }
            else
            {
                await gridFs.CopyToAsync(resp.Body);
            }
        }
    }
}
