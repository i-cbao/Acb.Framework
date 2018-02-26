using Acb.Configuration;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Core.Modules;

namespace Acb.Framework.Logging
{
    [DependsOn(typeof(ConfigurationModule))]
    public class LogginModule : DModule
    {
        private static void SetTcpAddpend()
        {
            var tcpAppender = Log4NetDefaultConfig.TcpAppender();
            if (tcpAppender == null) return;
            LogManager.RemoveAdapter(typeof(TcpAppender));
            LogManager.AddAdapter(new TcpLogAdapter(tcpAppender));
        }

        public override void Initialize()
        {
            SetTcpAddpend();
            ConfigHelper.Instance.ConfigChanged += obj =>
            {
                SetTcpAddpend();
            };
            base.Initialize();
        }
    }
}
