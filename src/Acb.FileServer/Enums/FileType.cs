using System.ComponentModel;

namespace Acb.FileServer.Enums
{
    public enum FileType : byte
    {
        /// <summary> 所有类型 </summary>
        All = 0,

        /// <summary> 图片类型 </summary>
        [Description("图片")]
        Image = 1,

        /// <summary> 视频类型 </summary>
        [Description("视频")]
        Video = 2,

        /// <summary> 附件类型 </summary>
        [Description("附件")]
        Attach = 3,

        /// <summary> 认证文件，私密文件 </summary>
        [Description("认证")]
        Authentication = 4,

        /// <summary> 压缩文件，需解压 </summary>
        [Description("压缩")]
        ZipFile = 5,

        /// <summary> 音频类型 </summary>
        [Description("音频")]
        Audio = 6
    }
}
