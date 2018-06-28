using Acb.Core.Dependency;
using System;
using System.Data;

namespace Acb.WebApi.Test.Connections
{
    public interface IConnectionStruct : IScopedDependency, IDisposable
    {
        IDbConnection Connection(string name = null);
    }
}
