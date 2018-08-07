using System;

namespace Acb.EventBus
{
    /// <summary> 事件重试异常(返回该异常，消息将重试) </summary>
    public class EventRetryException : Exception
    {
    }
}
