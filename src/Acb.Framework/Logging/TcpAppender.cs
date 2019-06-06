using Acb.Core;
using Acb.Core.Extensions;
using log4net;
using log4net.Appender;
using log4net.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ILogger = Acb.Core.Logging.ILogger;
using LogManager = Acb.Core.Logging.LogManager;

namespace Acb.Framework.Logging
{
    internal class TcpAppender : AppenderSkeleton
    {
        private int _remotePort;
        private readonly ILogger _logger;

        public TcpAppender()
        {
            _logger = LogManager.Logger<TcpAppender>();
        }

        public IPAddress RemoteAddress { get; set; }

        public int RemotePort
        {
            get => _remotePort;
            set
            {
                if (value < 0 || value > ushort.MaxValue)
                {
                    _logger.Warn($"The RemotePort {value} is less than 0 or greater than {ushort.MaxValue}.");
                    return;
                }

                _remotePort = value;
            }
        }

        public Encoding Encoding { get; set; } = Encoding.Unicode;

        protected TcpClient Client { get; set; }

        protected IPEndPoint RemoteEndPoint { get; set; }

        protected override bool RequiresLayout => true;

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            if (RemoteAddress == null)
            {
                _logger.Warn("The required property 'Address' was not specified.");
                return;
            }

            if (RemotePort < 0 || RemotePort > ushort.MaxValue)
                _logger.Warn($"The RemotePort {RemotePort} is less than 0 or greater than {ushort.MaxValue}.");
            RemoteEndPoint = new IPEndPoint(RemoteAddress, RemotePort);
            InitializeClientConnection();
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                if (Client == null)
                    InitializeClientConnection();
                if (Client == null)
                    return;
                var ntwStream = Client.GetStream();
                var dict = new Dictionary<string, object>
                {
                    {"date", loggingEvent.TimeStamp},
                    {"level", loggingEvent.Level.DisplayName},
                    {"site", GlobalContext.Properties["LogSite"]},
                    {"logger", loggingEvent.LoggerName},
                    {"exception", loggingEvent.ExceptionObject?.Format()},
                    {"host", Utils.GetLocalIp()}
                };
                if (loggingEvent.MessageObject != null)
                {
                    if (loggingEvent.MessageObject.GetType().IsSimpleType())
                    {
                        dict.Add("msg", loggingEvent.MessageObject);
                    }
                    else
                    {
                        foreach (var item in loggingEvent.MessageObject.ToDictionary())
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
                //var aa = RenderLoggingEvent(loggingEvent).ToCharArray();
                //var bytes = Encoding.GetBytes(RenderLoggingEvent(loggingEvent).ToCharArray());
                ntwStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                _logger.Warn(
                    $"Unable to send logging event to remote host {RemoteAddress} on port {RemotePort}.{ex.Format()}");
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (Client == null)
                return;
            Client.Dispose();
            Client = null;
        }

        protected virtual void InitializeClientConnection()
        {
            try
            {
                Client = new TcpClient(RemoteAddress.ToString(), RemotePort);
                _logger.Info($"开启分布式日志组件:{RemoteAddress}:{RemotePort}");
            }
            catch (Exception ex)
            {
                _logger.Warn($"Could not initialize the UdpClient connection on port {RemotePort}.{ex.Format()}");
                Client = null;
            }
        }
    }

}
