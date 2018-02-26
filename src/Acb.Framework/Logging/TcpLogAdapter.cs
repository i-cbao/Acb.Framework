using Acb.Core.Helper;
using Acb.Core.Logging;
using log4net.Appender;
using log4net.Config;

namespace Acb.Framework.Logging
{
    public class TcpLogAdapter : LoggerAdapterBase
    {
        private static string _logRepository;
        /// <inheritdoc />
        /// <summary>
        /// 初始化一个<see cref="T:Acb.Framework.Logging.Log4NetAdapter" />类型的新实例
        /// </summary>k
        public TcpLogAdapter(IAppender appender)
        {
            _logRepository = $"tcp_append_{IdentityHelper.Guid16}";
            var repository = log4net.LogManager.CreateRepository(_logRepository);
            BasicConfigurator.Configure(repository, appender);
        }

        protected override ILog CreateLogger(string name)
        {
            var log = log4net.LogManager.GetLogger(_logRepository, name);
            return new Log4NetLog(log);
        }
    }
}
