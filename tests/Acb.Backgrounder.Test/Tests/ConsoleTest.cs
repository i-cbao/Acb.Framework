using Acb.Core.Logging;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Acb.Backgrounder.Test.Tests
{
    public abstract class ConsoleTest
    {
        protected readonly ILogger Logger;

        protected ConsoleTest()
        {
            Logger = LogManager.Logger(GetType());
        }

        public virtual void OnUseServiceProvider(IServiceProvider provider) { }

        public virtual void OnMapServiceCollection(IServiceCollection services) { }

        public virtual void OnUseServices(IContainer provider) { }

        public virtual void OnMapServices(ContainerBuilder builder) { }

        public virtual void OnCommand(string cmd, IContainer provider) { }

        public virtual void OnShutdown() { }
    }
}
