using Acb.Core.Extensions;
using Acb.Core.Timing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Acb.Core.Logging
{
    /// <summary> TCP远程日志服务 </summary>
    public class TcpRemoteLogger : IRemoteLogger
    {
        private readonly ILogger _logger;
        private readonly string _host;
        private readonly int _port;
        private TcpClient _client;

        /// <summary> 编码 </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary> 构造函数 </summary>
        public TcpRemoteLogger()
        {
            _logger = LogManager.Logger<TcpRemoteLogger>();
        }

        /// <summary> 构造函数 </summary>
        /// <param name="host">主机名</param>
        /// <param name="port">端口号</param>
        public TcpRemoteLogger(string host, int port) : this()
        {
            _host = host;
            _port = port;
        }

        private void CreateClient()
        {
            var host = _host;
            var port = _port;
            try
            {
                if (string.IsNullOrWhiteSpace(host))
                    host = "tcpLogger:address".Config<string>();
                if (port <= 0)
                    port = "tcpLogger:port".Config<int>();
                _client = new TcpClient(host, port);
                _logger.Debug($"创建TCP远程日志组件：{host}:{port}.");
            }
            catch (Exception ex)
            {
                _logger.Warn($"Could not initialize the UdpClient connection on  {host}:{port}.", ex);
                _client = null;
            }
        }

        /// <inheritdoc />
        /// <summary> 记录日志 </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        /// <param name="ex"></param>
        /// <param name="date"></param>
        /// <param name="logger"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public async Task Logger(object msg, LogLevel level, Exception ex = null, DateTime? date = null, string logger = null, string site = null)
        {
            try
            {
                if (_client == null)
                    CreateClient();
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
            catch (Exception e)
            {
                _logger.Warn($"Unable to send logging event to remote host {_host} on port {_port}.", e);
            }
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
