using Newtonsoft.Json;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Acb.MicroService.PureClient
{
    public enum ServiceProtocol
    {
        Tcp,
        Http,
        Ws
    }

    public class ServiceAddress
    {
        public IPAddress Ip { get; set; }
        public ServiceProtocol Protocol { get; set; } = ServiceProtocol.Http;
        public string Host { get; set; }
        public int Port { get; set; }
        /// <summary> 对外注册的服务地址(ip或DNS) </summary>
        public string Service { get; set; }

        public ServiceAddress() { }

        public ServiceAddress(string host, int port)
        {
            if (HasProtocol(host))
            {
                var uri = new Uri(host);
                if (Enum.TryParse<ServiceProtocol>(uri.Scheme, true, out var protocol))
                {
                    Protocol = protocol;
                }
                Host = uri.Host;
            }
            else
            {
                Host = host;
            }
            Port = port;
        }

        private static bool HasProtocol(string host)
        {
            return host.IndexOf("//", StringComparison.Ordinal) > 0;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string Address()
        {
            var host = string.IsNullOrWhiteSpace(Service) ? Host : Service;
            if (HasProtocol(host)) return host;
            return $"{Protocol.ToString().ToLower()}://{host}";
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Host))
                Host = Ip.ToString();
            if (Port == 80 || Port == 443)
                return Address();
            return $"{Address()}:{Port}";
        }

        private const string IpRegex = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

        public EndPoint ToEndPoint(bool isServer = true)
        {
            var service = isServer ? Host : Service;
            if (string.IsNullOrWhiteSpace(service) || service == "localhost")
            {
                return new IPEndPoint(IPAddress.Any, Port);
            }

            if (Regex.IsMatch(service, IpRegex))
                return new IPEndPoint(IPAddress.Parse(service), Port);
            return new DnsEndPoint(service, Port);
        }
    }
}
