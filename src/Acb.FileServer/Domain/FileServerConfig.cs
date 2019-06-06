using System;
using System.Xml.Serialization;

namespace Acb.FileServer.Domain
{
    [Serializable]
    public class FileServerConfig
    {
        /// <summary> 站点 </summary>
        public string Site { get; set; }

        /// <summary> 基础文件路径 </summary>
        public string Dir { get; set; }

        /// <summary> 默认图片 </summary>
        public string DefaultImage { get; set; }

        /// <summary> 单目录最多文件数量 </summary>
        public int MaxFileCount { get; set; }

        public int SaveType { get; set; }

        /// <summary> 图片文件 </summary>
        public FileTypeLimit Image { get; set; }

        /// <summary> 视频文件 </summary>
        public FileTypeLimit Video { get; set; }

        /// <summary> 音频文件 </summary>
        public FileTypeLimit Audio { get; set; }

        /// <summary> 附件 </summary>
        public FileTypeLimit Attach { get; set; }

    }

    [Serializable]
    public class FileTypeLimit
    {
        /// <summary> 允许的扩展名列表 </summary>
        [XmlIgnore]
        public string[] Exts { get; set; }

        public string ExtsAttr
        {
            get { return string.Join(";", Exts); }
            set { Exts = string.IsNullOrWhiteSpace(value) ? new string[] { } : value.Split(';'); }
        }

        /// <summary> 上传大小限制(kb) </summary>
        public int MaxSize { get; set; }
    }
}