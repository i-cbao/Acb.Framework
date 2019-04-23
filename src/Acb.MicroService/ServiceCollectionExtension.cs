using Acb.Core.Extensions;
using Acb.Core.Helper.Http;
using Acb.Core.Message;
using Acb.Core.Message.Codec;
using Acb.MicroService.Message;
using Acb.MicroService.Router;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Acb.MicroService
{
    public static class ServiceCollectionExtension
    {
        private const string MicroSreviceKey = "micro_service";

        /// <summary> 服务命名 </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string ServiceName(this Assembly assembly)
        {
            var assName = assembly.GetName();
            return $"{assName.Name}_v{assName.Version.Major}";
        }

        /// <summary> 使用Json编解码器 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddJsonCodec(this IServiceCollection services)
        {
            services.AddSingleton<IMessageCodec, JsonMessageCodec>();
            return services;
        }

        /// <summary> 使用MessagePack编解码器 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMessagePackCodec(this IServiceCollection services)
        {
            services.AddSingleton<IMessageCodec, MessagePackCodec>();
            return services;
        }

        /// <summary> 使用ProtoBufffer编解码器 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddProtoBufferCodec(this IServiceCollection services)
        {
            services.AddSingleton<IMessageCodec, ProtoBufferCodec>();
            return services;
        }

        public static IServiceCollection AddMicroRouter(this IServiceCollection services)
        {
            services.AddSingleton<IServiceRouter>(provider =>
            {
                var config = MicroSreviceKey.Config<MicroServiceConfig>();
                switch (config.Register)
                {
                    case RegisterType.Consul:
                        return new ConsulRouter(config);
                    case RegisterType.Redis:
                        var codec = provider.GetService<IMessageCodec>();
                        return new RedisRouter(codec, config);
                    default:
                        return new ConsulRouter(config);
                }
            });
            return services;
        }

        /// <summary> 使用Consul服务注册与发现 </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsulRouter(this IServiceCollection services,
            Action<MicroServiceConfig> configAction = null)
        {
            services.AddSingleton<IServiceRouter>(provider =>
            {
                var config = MicroSreviceKey.Config<MicroServiceConfig>();
                configAction?.Invoke(config);
                return new ConsulRouter(config);
            });
            return services;
        }

        /// <summary> 使用Redis服务注册与发现 </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedisRouter(this IServiceCollection services,
            Action<MicroServiceConfig> configAction = null)
        {
            services.AddSingleton<IServiceRouter>(provider =>
            {
                var config = MicroSreviceKey.Config<MicroServiceConfig>();
                configAction?.Invoke(config);
                var codec = provider.GetService<IMessageCodec>();
                return new RedisRouter(codec, config);
            });
            return services;
        }

        internal static async Task<QueryResult<CatalogService[]>> Service(this ConsulClient client, string name,
            object[] tags, CancellationToken ct = default(CancellationToken))
        {
            var url = new Uri(client.Config.Address, $"/v1/catalog/service/{name}").AbsoluteUri;
            var ps = new List<string>();
            if (!string.IsNullOrWhiteSpace(client.Config.Token))
            {
                ps.Add($"token={client.Config.Token.UrlEncode()}");
            }

            ps.AddRange(tags.Where(t => t != null).Select(tag => $"tag={tag.ToString().UrlEncode()}"));

            if (ps.Any())
            {
                url += "?" + string.Join("&", ps);
            }

            var watcher = Stopwatch.StartNew();
            var resp = await HttpHelper.Instance.RequestAsync(HttpMethod.Get, new HttpRequest(url));
            watcher.Stop();
            var result =
                new QueryResult<CatalogService[]> { StatusCode = resp.StatusCode, RequestTime = watcher.Elapsed };

            if (!resp.IsSuccessStatusCode)
                return result;
            var content = await resp.Content.ReadAsStringAsync();
            result.Response = JsonConvert.DeserializeObject<CatalogService[]>(content);
            return result;

        }
    }
}
