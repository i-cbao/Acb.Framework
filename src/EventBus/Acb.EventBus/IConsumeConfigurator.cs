using System;
using System.Collections.Generic;
using Autofac;

namespace Acb.EventBus
{
    public interface IConsumeConfigurator
    {
        IContainer Provider { get; set; }
        void Configure(List<Type> consumers);
    }
}
