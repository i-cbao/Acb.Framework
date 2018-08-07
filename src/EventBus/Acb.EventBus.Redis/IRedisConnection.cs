using StackExchange.Redis;

namespace Acb.EventBus.Redis
{
    public interface IRedisConnection
    {
        ISubscriber GetSubscriber();
    }
}
