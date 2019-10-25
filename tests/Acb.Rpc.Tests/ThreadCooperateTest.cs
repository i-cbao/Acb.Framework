using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Acb.Rpc.Tests
{
    /// <summary> 线程协作 </summary>
    [TestClass]
    public class ThreadCooperateTest
    {
        // Java
        // LockSupport park unpark
        // ReentrantLock Condition
        // synchronized notify wait

        private enum WorkType { A, B }

        private static Task ThreadAction(WorkType type, Action beforeAction = null, Action afterAction = null)
        {
            return new Task(() =>
            {
                for (var i = 0; i < 26; i++)
                {
                    beforeAction?.Invoke();
                    Console.Write(type == WorkType.A ? $"{i + 1}" : $"{(char)(i + 65)}");
                    afterAction?.Invoke();
                }
            });
        }

        [TestMethod]
        public void SpinLockTest()
        {
            //自旋锁
            var current = WorkType.A;
            var taskA = ThreadAction(WorkType.A, () =>
            {
                //自旋
                while (current != WorkType.A) { }
            }, () => { current = WorkType.B; });

            var taskB = ThreadAction(WorkType.B, () =>
            {
                //自旋
                while (current != WorkType.B) { }
            }, () => { current = WorkType.A; });
            taskA.Start();
            taskB.Start();
            Task.WaitAll(taskA, taskB);
        }

        [TestMethod]
        public void BlockingQueueTest()
        {
            // 阻塞队列
            var queueA = new BlockingCollection<string>(1);
            var queueB = new BlockingCollection<string>(1);
            var taskA = ThreadAction(WorkType.A, () =>
            {
            }, () =>
            {
                //推送队列A
                queueA.Add("ok");
                //等待队列B输出
                queueB.Take();
            });

            var taskB = ThreadAction(WorkType.B, () =>
            {
                //等待队列A输出
                queueA.Take();
            }, () =>
            {
                //推送队列B
                queueB.Add("ok");
            });
            taskA.Start();
            taskB.Start();
            Task.WaitAll(taskA, taskB);
        }

        [TestMethod]
        public void AutoResetEventTest()
        {
            // 自动重置事件
            var workerA = new AutoResetEvent(false);
            var workerB = new AutoResetEvent(false);
            var taskA = ThreadAction(WorkType.A, () =>
            {
            }, () =>
            {
                //释放B
                workerB.Set();
                //挂起A
                workerA.WaitOne();
            });

            var taskB = ThreadAction(WorkType.B, () =>
            {
                //挂起B
                workerB.WaitOne();
            }, () =>
            {
                //释放A
                workerA.Set();
            });
            taskA.Start();
            taskB.Start();
            Task.WaitAll(taskA, taskB);
            workerA.Close();
            workerB.Close();
        }
    }
}
