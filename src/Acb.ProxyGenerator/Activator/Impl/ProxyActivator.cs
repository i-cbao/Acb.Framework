using System;
using System.Threading.Tasks;

namespace Acb.ProxyGenerator.Activator.Impl
{
    internal sealed class ProxyActivator : IProxyActivator
    {
        private readonly IProxyContextFactory _contextFactory;
        private readonly IProxyBuilderFactory _builderFactory;

        public ProxyActivator(IProxyContextFactory contextFactory, IProxyBuilderFactory builderFactory)
        {
            _contextFactory = contextFactory;
            _builderFactory = builderFactory;
        }

        public TResult Invoke<TResult>(ProxyActivatorContext activatorContext)
        {
            var context = _contextFactory.CreateContext(activatorContext);
            try
            {
                var builder = _builderFactory.Create(context);
                var task = builder.Build()(context);
                if (task.IsFaulted && task.Exception != null)
                    throw context.ProxyException(task.Exception.InnerException);
                if (!task.IsCompleted)
                {
                    // try to avoid potential deadlocks.
                    task.GetAwaiter().GetResult();
                    // task.GetAwaiter().GetResult();
                }
                return (TResult)context.ReturnValue;
            }
            catch (Exception ex)
            {
                throw context.ProxyException(ex);
            }
            finally
            {
                _contextFactory.ReleaseContext(context);
            }
        }

        public async Task<TResult> InvokeTask<TResult>(ProxyActivatorContext activatorContext)
        {
            var context = _contextFactory.CreateContext(activatorContext);
            try
            {
                var aspectBuilder = _builderFactory.Create(context);
                await aspectBuilder.Build()(context);
                var result = context.ReturnValue;
                if (result is Task<TResult> taskWithResult)
                {
                    return await taskWithResult;
                }
                else if (result is Task task)
                {
                    await task;
                    return default(TResult);
                }
                else
                {
                    throw context.ProxyException(new InvalidCastException(
                        $"Unable to cast object of type '{result.GetType()}' to type '{typeof(Task<TResult>)}'."));
                }
            }
            catch (Exception ex)
            {
                throw context.ProxyException(ex);
            }
            finally
            {
                _contextFactory.ReleaseContext(context);
            }
        }

        public async ValueTask<TResult> InvokeValueTask<TResult>(ProxyActivatorContext activatorContext)
        {
            var context = _contextFactory.CreateContext(activatorContext);
            try
            {
                var aspectBuilder = _builderFactory.Create(context);
                await aspectBuilder.Build()(context);
                return await (ValueTask<TResult>)context.ReturnValue;
            }
            catch (Exception ex)
            {
                throw context.ProxyException(ex);
            }
            finally
            {
                _contextFactory.ReleaseContext(context);
            }
        }
    }
}