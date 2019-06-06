namespace Acb.FileServer.ViewModels
{
    public class VUploadInput
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        /// <summary> 文件Base64编码 </summary>
        public string Base64 { get; set; }
    }
}
