using System;
using System.Threading;

namespace Acb.Core.Helper
{
    internal class ConfigCenterRefresh
    {
        private readonly Timer _timer;
        private ConfigCenterProvider _provider;
        private int _interval;

        private ConfigCenterRefresh()
        {
            _timer = new Timer(OnTimerElapsed);
        }

        public static ConfigCenterRefresh Instance =>
            Singleton<ConfigCenterRefresh>.Instance ??
            (Singleton<ConfigCenterRefresh>.Instance = new ConfigCenterRefresh());

        public void Start(int seconds, ConfigCenterProvider provider)
        {
            _interval = seconds;
            _provider = provider;
            _timer.Change(TimeSpan.FromMilliseconds(1), Timeout.InfiniteTimeSpan);
        }

        public void SetInterval(int seconds)
        {
            _interval = seconds;
        }

        private void OnTimerElapsed(object sender)
        {
            _provider.Reload();
            _timer.Change(TimeSpan.FromSeconds(_interval), Timeout.InfiniteTimeSpan);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
