namespace Acb.ConfigCenter.ViewModels
{
    public class SecurityDto
    {
        /// <summary> 开启认证 </summary>
        public bool Enabled { get; set; }
        /// <summary> 开启认证之后是否允许匿名获取 </summary>
        public bool Get { get; set; }
        /// <summary> 账号 </summary>
        public string Account { get; set; }
        /// <summary> 密码 </summary>
        public string Password { get; set; }
    }
}
