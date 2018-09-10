using RabbitMQ.Client;
using System.Collections.Generic;

namespace Acb.RabbitMq
{
    public static class RabbitMqExtensions
    {
        /// <summary> 申明队列并设置死信队列 </summary>
        /// <param name="channel"></param>
        /// <param name="queue"></param>
        /// <param name="exchange"></param>
        /// <param name="routeKey"></param>
        public static void DeclareWithDlx(this IModel channel, string queue, string exchange, string routeKey)
        {
            var dlxQueue = $"~dlx_{queue}";
            var args = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", exchange},
                {"x-dead-letter-routing-key", dlxQueue}
            };
            //死信队列
            //1.消息被拒绝，并且设置ReQueue参数false；
            //2.消息过期；
            //3.队列打到最大长度；
            channel.QueueDeclare(dlxQueue, true, false);
            channel.QueueBind(dlxQueue, exchange, dlxQueue, null);

            //声明队列
            channel.QueueDeclare(queue, true, false, false, args);
            channel.QueueBind(queue, exchange, routeKey);
        }

        /// <summary> 延迟发布 </summary>
        /// <param name="channel"></param>
        /// <param name="exchange"></param>
        /// <param name="routeKey"></param>
        /// <param name="body"></param>
        /// <param name="prop"></param>
        /// <param name="delay"></param>
        public static void DelayPublish(this IModel channel, string exchange, string routeKey, byte[] body, long delay,
            IBasicProperties prop = null)
        {
            //延迟队列
            var delayQueue = $"~delay_{routeKey}";
            var args = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", exchange},
                {"x-dead-letter-routing-key", routeKey}
            };
            //死信队列
            //1.消息被拒绝，并且设置ReQueue参数false；
            //2.消息过期；
            //3.队列打到最大长度；
            channel.QueueDeclare(delayQueue, true, false, true, args);
            channel.QueueBind(delayQueue, exchange, delayQueue, null);
            if (prop == null)
                prop = channel.CreateBasicProperties();
            prop.Expiration = delay.ToString();
            channel.BasicPublish(exchange, delayQueue, prop, body);
        }
    }
}
