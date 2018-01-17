using Acb.Core.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Acb.Configuration
{
    internal class RefreshHelper
    {
        [ThreadStatic]
        private static bool _abort;

        [ThreadStatic]
        private static bool _isRunning;

        public static void Start(int seconds)
        {
            if (_isRunning)
                return;
            _abort = false;
            Task.Run(() =>
            {
                _isRunning = true;
                while (true)
                {
                    Thread.CurrentThread.Join(TimeSpan.FromSeconds(seconds));
                    ConfigHelper.Instance.Reload();
                    if (_abort)
                        break;
                }
            });
        }

        public static void Stop()
        {
            _isRunning = false;
            _abort = true;
        }
    }
}
