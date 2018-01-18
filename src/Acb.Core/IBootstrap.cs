using System;

namespace Acb.Core
{
    /// <summary> 启动类接口 </summary>
    public interface IBootstrap : IDisposable
    {
        void Initialize();
    }
}
