namespace Acb.MicroService
{
    public class EndpointConfiguration
    {
        /// <summary> host </summary>
        public string Host { get; set; }
        /// <summary> port </summary>
        public int? Port { get; set; }
        /// <summary> 方案：http/https </summary>
        public string Scheme { get; set; }
        public string StoreName { get; set; }
        public string StoreLocation { get; set; }
        public string FilePath { get; set; }
        public string Password { get; set; }
    }
}
