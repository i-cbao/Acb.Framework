using Acb.Core.Dependency;
using System;
using System.Threading;

namespace Acb.Core.Helper
{
    internal class ConfigCenterRefresh : ISingleDependency
    {
        private Timer _timer;
        private ConfigCenterProvider _provider;
        private int _interval;

        public void Start(int seconds, ConfigCenterProvider provider)
        {
            Stop();
            _timer = new Timer(OnTimerElapsed);
            _interval = seconds;
            _provider = provider;
            _timer.Change(TimeSpan.FromSeconds(_interval), Timeout.InfiniteTimeSpan);
        }

        private void OnTimerElapsed(object sender)
        {
            _provider.Reload();
            _timer.Change(TimeSpan.FromSeconds(_interval), Timeout.InfiniteTimeSpan);
        }

        public void Stop()
        {
            _timer?.Dispose();
        }
    }
}
