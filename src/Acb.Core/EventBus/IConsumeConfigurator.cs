using System;
using System.Collections.Generic;
using Acb.Core.Dependency;

namespace Acb.Core.EventBus
{
    public interface IConsumeConfigurator : ISingleDependency
    {
        void Configure(List<Type> consumers);
    }
}
