using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Acb.MicroService.PureClient.Router.Implementation
{
    public class ConsulClient
    {
        private readonly Uri _host;
        private readonly string _token;
        private readonly IHttpClientFactory _clientFactory;

        public ConsulClient(Uri host, string token = null)
        {
            _clientFactory = Extensions.GetService<IHttpClientFactory>();
            _host = host;
            _token = token;
        }

        public async Task<List<ServiceAddress>> GetServices(string serviceName, string tag = null)
        {
            var querys = new List<string>();
            if (!string.IsNullOrWhiteSpace(tag))
                querys.Add($"tag={HttpUtility.UrlEncode(tag)}");
            if (!string.IsNullOrWhiteSpace(_token))
                querys.Add($"token={HttpUtility.UrlEncode(_token)}");
            var url = $"v1/health/service/{serviceName}";
            if (querys.Any())
                url += $"?{string.Join("&", querys.ToArray())}";
            var uri = new Uri(_host, url);
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            var client = _clientFactory.CreateClient();
            var resp = await client.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
                return new List<ServiceAddress>();
            var list = new List<ServiceAddress>();
            var html = await resp.Content.ReadAsStringAsync();
            var nodes = JsonConvert.DeserializeObject<List<ServiceResult>>(html);
            foreach (var node in nodes)
            {
                var address = node.Service.Address;
                var port = node.Service.Port;
                var reg = new Regex("^http://micro-([a-z]+)$", RegexOptions.IgnoreCase);
                var match = reg.Match(address);
                if (match.Success)
                {
                    var name = match.Groups[1].Value;
                    address = $"http://micro.icb/{name}/";
                    port = 80;
                }

                list.Add(new ServiceAddress(address, port)
                {
                    Service = address
                });
            }

            return list;
        }

        private class ServiceResult
        {
            public ServiceItem Service { get; set; }

            public ServiceResult()
            {
                Service = new ServiceItem();
            }
        }

        private class ServiceItem
        {
            public string Address { get; set; }
            public int Port { get; set; }
        }
    }
}
