using Acb.Core.Logging;
using log4net.Appender;
using log4net.Core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Acb.Framework.Logging
{
    internal class TcpAppender : AppenderSkeleton
    {
        private int _remotePort;
        private readonly Core.Logging.ILogger _logger = LogManager.Logger<TcpAppender>();

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
                var ntwStream = Client.GetStream();
                //var aa = RenderLoggingEvent(loggingEvent).ToCharArray();
                var bytes = Encoding.GetBytes(RenderLoggingEvent(loggingEvent).ToCharArray());
                ntwStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                _logger.Warn($"Unable to send logging event to remote host {RemoteAddress} on port {RemotePort}.", ex);
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
            }
            catch (Exception ex)
            {
                _logger.Warn($"Could not initialize the UdpClient connection on port {RemotePort}.", ex);
                Client = null;
            }
        }
    }

}
