using System;
using System.Threading;
using System.Threading.Tasks;

namespace Acb.Core.Extensions
{
    public static class TaskExtensions
    {
        private static IDisposable Enter()
        {
            var context = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);
            return new Disposable(context);
        }

        private struct Disposable : IDisposable
        {
            private readonly SynchronizationContext _synchronizationContext;

            public Disposable(SynchronizationContext synchronizationContext)
            {
                _synchronizationContext = synchronizationContext;
            }

            public void Dispose() =>
                SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
        }

        /// <summary> 同步执行异步方法(防止死锁) </summary>
        /// <param name="task"></param>
        public static void SyncRun(this Task task)
        {
            using (Enter())
            {
                task.GetAwaiter().GetResult();
            }
        }

        /// <summary> 同步执行异步方法(防止死锁) </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static T SyncRun<T>(this Task<T> task)
        {
            using (Enter())
            {
                return task.GetAwaiter().GetResult();
            }
        }
    }
}
