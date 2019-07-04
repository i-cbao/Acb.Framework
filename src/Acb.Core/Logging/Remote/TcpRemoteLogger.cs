using Acb.Core.Extensions;
using Acb.Core.Timing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Acb.Core.Logging.Remote
{
    /// <summary> TCP远程日志服务 </summary>
    public class TcpRemoteLogger : IRemoteLogger
    {
        private readonly ILogger _logger;
        private static TcpClient _client;

        /// <summary> 编码 </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary> 构造函数 </summary>
        public TcpRemoteLogger()
        {
            _logger = LogManager.Logger<TcpRemoteLogger>();
            var config = RemoteLoggerConfig.Config();
            Host = config.Address;
            Port = config.Port;
            CreateClient();
        }

        /// <summary> 构造函数 </summary>
        /// <param name="host">主机名</param>
        /// <param name="port">端口号</param>
        public TcpRemoteLogger(string host, int port) : this()
        {
            Host = host;
            Port = port;
        }

        private void CreateClient()
        {
            try
            {
                _client = new TcpClient(Host, Port);
                _logger.Debug($"创建TCP远程日志组件：{Host}:{Port}.");
            }
            catch (Exception ex)
            {
                _logger.Warn($"Could not initialize the UdpClient connection on  {Host}:{Port}.{ex.Message}");
                _client = null;
            }
        }

        private async Task InternalLog(object msg, LogLevel level, Exception ex = null, DateTime? date = null,
            string logger = null, string site = null)
        {
            Thread.Sleep(5000);
            if (_client == null || !_client.Connected)
                CreateClient();
            if (_client == null)
                return;
            var ntwStream = _client.GetStream();
            var dict = new Dictionary<string, object>
            {
                {"date", date ?? Clock.Now},
                {"level", level.ToString()},
                {"site", string.IsNullOrWhiteSpace(site) ? "site".Config(site) : site},
                {"logger", logger},
                {"exception", ex?.Format()},
                {"host", Utils.GetLocalIp()}
            };
            if (msg != null)
            {
                if (msg.GetType().IsSimpleType())
                {
                    dict.Add("msg", msg);
                }
                else
                {
                    foreach (var item in msg.ToDictionary())
                    {
                        if (item.Value == null || item.Value.Equals(string.Empty)) continue;
                        var key = item.Key.ToCamelCase();
                        //替换关键的message键
                        if (key == "message") key = "msg";
                        dict.AddOrUpdate(key, item.Value);
                    }
                }
            }

            //需要添加\n
            var bytes = Encoding.GetBytes(JsonConvert.SerializeObject(dict) + "\n");
            await ntwStream.WriteAsync(bytes, 0, bytes.Length);
        }

        public string Host { get; }

        public int Port { get; }

        /// <inheritdoc />
        /// <summary> 记录日志 </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="ex"></param>
        /// <param name="date"></param>
        /// <param name="logger"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public void Logger(object msg, LogLevel level, Exception ex = null, DateTime? date = null, string logger = null, string site = null)
        {
            Task.Run(async () =>
            {
                try
                {
                    await InternalLog(msg, level, ex, date, logger, site);
                }
                catch (Exception e)
                {
                    _logger.Warn($"Unable to send logging event to remote host {Host} on port {Port}.{e.Message},{e.Format()}");
                }
            });
        }

        /// <inheritdoc />
        /// <summary> 释放资源 </summary>
        public void Dispose()
        {
            _logger.Debug("TcpRemoteLogger Dispose");
            _client?.Dispose();
        }
    }
}
