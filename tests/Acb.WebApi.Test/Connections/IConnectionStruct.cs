using Acb.Core.Dependency;
using System;
using System.Data;

namespace Acb.WebApi.Test.Connections
{
    public interface IConnectionStruct : ILifetimeDependency, IDisposable
    {
        IDbConnection Connection(string name = null);
    }
}
