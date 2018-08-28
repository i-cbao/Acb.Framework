using Acb.Core.Dependency;
using System;
using System.Data;

namespace Acb.Core.Domain
{
    /// <summary> 业务单元操作接口 </summary>
    public interface IUnitOfWork : IDependency, IDisposable
    {
        /// <summary> 获取数据库连接 </summary>
        IDbConnection Conntection { get; }

        /// <summary> 当前事务 </summary>
        IDbTransaction Transaction { get; }

        /// <summary> 是否开启了事务 </summary>
        bool IsTransaction { get; }

        /// <summary> 获取当前数据库事务 </summary>
        /// <returns></returns>
        void BeginTransaction(Action action, IsolationLevel? level = null);

        /// <summary> 保存修改 </summary>
        T BeginTransaction<T>(Func<T> func, IsolationLevel? level = null);
    }
}
