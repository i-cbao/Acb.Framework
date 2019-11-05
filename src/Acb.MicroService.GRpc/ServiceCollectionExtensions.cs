using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Acb.MicroService.GRpc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGRpc(this IServiceCollection services)
        {
            var channel = new Channel("", 500, ChannelCredentials.Insecure);

            return services;
        }

        public static IServiceCollection AddGRpcClient(this IServiceCollection services)
        {

            return services;
        }
    }
}
