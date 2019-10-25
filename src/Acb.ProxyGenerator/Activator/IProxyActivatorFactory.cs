namespace Acb.ProxyGenerator.Activator
{
    /// <summary> 代理执行器工厂 </summary>
    public interface IProxyActivatorFactory
    {
        /// <summary> 创建代理执行器 </summary>
        IProxyActivator Create();
    }
}