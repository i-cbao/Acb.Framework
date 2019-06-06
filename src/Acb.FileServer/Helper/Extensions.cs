using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.FileServer.Domain;
using Acb.FileServer.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Acb.FileServer.Helper
{
    /// <summary> 扩展辅助 </summary>
    public static class Extensions
    {
        /// <summary> 获取绝对路径uri </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetAbsoluteUri(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }

        /// <summary> 相对路径 </summary>
        /// <param name="context"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapPath(this HttpContext context, string path)
        {
            var env = context.RequestServices.GetService<IHostingEnvironment>();
            return Path.Combine(env.WebRootPath, path);
        }

        /// <summary> 是否是图片格式 </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsImage(this string ext)
        {
            if (string.IsNullOrWhiteSpace(ext))
                return false;
            return new[] { ".jpg", ".png", ".jpeg", ".Gif" }.Contains(ext.ToLower());
        }

        /// <summary> url解析图片大小 </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static (string, int, int) ImageSize(this string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return (filename, 0, 0);
            var match = Regex.Match(filename, Constants.ImageUrlRegex, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var width = match.Groups["w"].Value.CastTo(0);
                var height = match.Groups["h"].Value.CastTo(0);
                var name = filename.Replace(match.Groups[1].Value, string.Empty);
                return (name, width, height);
            }

            return (filename, 0, 0);
        }

        /// <summary> 文件大小 </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string FileSize(this int size)
        {
            if (size < 1024)
                return $"{size}K";
            return size < 1024 * 1024 ? $"{size / 1024M:F} M" : $"{size / (1024 * 1024M):F}G";
        }

        /// <summary> 文件类型默认扩展名 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string DefaultExtOfFileType(this FileType type)
        {
            switch (type)
            {
                case FileType.Image:
                    return ".jpg";
                case FileType.Audio:
                    return ".mp3";
                case FileType.Video:
                    return ".mp4";
                case FileType.Attach:
                    return ".zip";
                default:
                    throw new BusiException("文件类型异常");
            }
        }

        /// <summary> 文件检测 </summary>
        /// <param name="path">uri或路径</param>
        /// <param name="contentLength"></param>
        /// <param name="filetype">文件类型</param>
        /// <returns></returns>
        public static void CheckExtAndSize(this FileType filetype, long contentLength, string path = null)
        {
            if (contentLength == 0)
                throw new BusiException("文件数据丢失！");
            if (!Enum.IsDefined(typeof(FileType), filetype) || filetype == FileType.ZipFile || filetype == FileType.All)
                throw new BusiException("不支持的文件类型！");
            FileTypeLimit limit;
            var config = Constants.Config;
            switch (filetype)
            {
                case FileType.Image:
                    limit = config.Image;
                    break;
                case FileType.Video:
                    limit = config.Video;
                    break;
                case FileType.Audio:
                    limit = config.Audio;
                    break;
                default:
                    limit = config.Attach;
                    break;
            }
            var ext = string.IsNullOrWhiteSpace(path) ? filetype.DefaultExtOfFileType() : Path.GetExtension(path);
            if (!limit.Exts.Contains(ext))
                throw new BusiException($"{ext}不是有效的{filetype.GetText()}格式文件！");
            if (limit.MaxSize * 1024 < contentLength)
                throw new BusiException($"文件大小超出了限制范围：不大于{FileSize(limit.MaxSize)}！！");
        }

        /// <summary> 是否是Base64字符 </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;
            var arr = str.Split(',');
            if (arr.Length == 2)
                str = arr[1];
            const string base64Pattern = "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";
            return Regex.IsMatch(str, base64Pattern);
        }

        #region 大写金额
        /// <summary> 人民币大写金额 </summary>
        /// <param name="value">人民币数字金额值</param>
        /// <returns>返回人民币大写金额</returns>
        public static string GetChineseMoney(this decimal value)
        {
            string capResult = "";
            string capValue = $"{value:f4}";       //格式化
            int dotPos = capValue.IndexOf(".", StringComparison.Ordinal);                     //小数点位置

            bool addMinus = value < 0;      //是否在结果中加"负"
            int beginPos = addMinus ? 1 : 0;                        //开始位置
            string capInt = capValue.Substring(beginPos, dotPos - beginPos);   //整数
            string capDec = capValue.Substring(dotPos + 1);         //小数

            if (dotPos > 0)
            {
                capResult = ConvertIntToUppercaseAmount(capInt) +
                            ConvertDecToUppercaseAmount(capDec, Convert.ToDouble(capInt) != 0 ? true : false);
            }
            else
            {
                capResult = ConvertIntToUppercaseAmount(capDec);
            }

            if (addMinus) capResult = "负" + capResult;
            //是否在结果中加"整"
            if (Convert.ToUInt32(capDec) == 0) capResult += "整";
            return capResult;
        }


        private const string Dxsz = "零壹贰叁肆伍陆柒捌玖";
        private const string Dxdw = "毫厘分角元拾佰仟萬拾佰仟亿拾佰仟萬兆拾佰仟萬亿京拾佰仟萬亿兆垓";
        private const string Scdw = "元拾佰仟萬亿京兆垓";

        /// <summary>
        /// 转换整数为大写金额
        /// 最高精度为垓，保留小数点后4位，实际精度为亿兆已经足够了，理论上精度无限制，如下所示：
        /// 序号:...30.29.28.27.26.25.24  23.22.21.20.19.18  17.16.15.14.13  12.11.10.9   8 7.6.5.4  . 3.2.1.0
        /// 单位:...垓兆亿萬仟佰拾        京亿萬仟佰拾       兆萬仟佰拾      亿仟佰拾     萬仟佰拾元 . 角分厘毫
        /// 数值:...1000000               000000             00000           0000         00000      . 0000
        /// 下面列出网上搜索到的数词单位：
        /// 元、十、百、千、万、亿、兆、京、垓、秭、穰、沟、涧、正、载、极
        /// </summary>
        /// <param name="capValue">整数值</param>
        /// <returns>返回大写金额</returns>
        private static string ConvertIntToUppercaseAmount(string capValue)
        {
            string currCap = "";    //当前金额
            string capResult = "";  //结果金额
            string currentUnit = "";//当前单位
            string resultUnit = ""; //结果单位
            int prevChar = -1;      //上一位的值
            int currChar = 0;       //当前位的值
            int posIndex = 4;       //位置索引，从"元"开始

            if (Convert.ToDouble(capValue) == 0)
                return "";
            for (int i = capValue.Length - 1; i >= 0; i--)
            {
                currChar = Convert.ToInt16(capValue.Substring(i, 1));
                if (posIndex > 30)
                {
                    //已超出最大精度"垓"。注：可以将30改成22，使之精确到兆亿就足够了
                    break;
                }
                else if (currChar != 0)
                {
                    //当前位为非零值，则直接转换成大写金额
                    currCap = Dxsz.Substring(currChar, 1) + Dxdw.Substring(posIndex, 1);
                }
                else
                {
                    //防止转换后出现多余的零,例如：3000020
                    switch (posIndex)
                    {
                        case 4: currCap = "元"; break;
                        case 8: currCap = "萬"; break;
                        case 12: currCap = "亿"; break;
                        case 17: currCap = "兆"; break;
                        case 23: currCap = "京"; break;
                        case 30: currCap = "垓"; break;
                        default: break;
                    }
                    if (prevChar != 0)
                    {
                        if (currCap != "")
                        {
                            if (currCap != "元") currCap += "零";
                        }
                        else
                        {
                            currCap = "零";
                        }
                    }
                }
                //对结果进行容错处理
                if (capResult.Length > 0)
                {
                    resultUnit = capResult.Substring(0, 1);
                    currentUnit = Dxdw.Substring(posIndex, 1);
                    if (Scdw.IndexOf(resultUnit, StringComparison.Ordinal) > 0)
                    {
                        if (Scdw.IndexOf(currentUnit, StringComparison.Ordinal) > Scdw.IndexOf(resultUnit, StringComparison.Ordinal))
                        {
                            capResult = capResult.Substring(1);
                        }
                    }
                }
                capResult = currCap + capResult;
                prevChar = currChar;
                posIndex += 1;
                currCap = "";
            }
            return capResult;
        }

        /// <summary>
        /// 转换小数为大写金额
        /// </summary>
        /// <param name="capValue">小数值</param>
        /// <param name="addZero">是否增加零位</param>
        /// <returns>返回大写金额</returns>
        private static string ConvertDecToUppercaseAmount(string capValue, bool addZero)
        {
            string currCap = "";
            string capResult = "";
            int prevChar = addZero ? -1 : 0;
            int currChar = 0;
            int posIndex = 3;

            if (Convert.ToInt16(capValue) == 0) return "";
            for (int i = 0; i < capValue.Length; i++)
            {
                currChar = Convert.ToInt16(capValue.Substring(i, 1));
                if (currChar != 0)
                {
                    currCap = Dxsz.Substring(currChar, 1) + Dxdw.Substring(posIndex, 1);
                }
                else
                {
                    if (Convert.ToInt16(capValue.Substring(i)) == 0)
                    {
                        break;
                    }
                    else if (prevChar != 0)
                    {
                        currCap = "零";
                    }
                }
                capResult += currCap;
                prevChar = currChar;
                posIndex -= 1;
                currCap = "";
            }
            return capResult;
        }
        #endregion
    }
}
