﻿using Acb.Core.Dependency;

namespace Acb.Core.EventBus
{
    /// <summary> 订阅适配器 </summary>
    public interface ISubscriptionAdapter : ISingleDependency
    {
        /// <summary> 订阅 </summary>
        void SubscribeAt();
    }
}
