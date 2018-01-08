﻿using System;
using System.Data;

namespace Acb.Dapper.Adapters
{
    public interface IDbConnectionAdapter
    {
        string ProviderName { get; }
        Type ConnectionType { get; }
        string FormatSql(string sql);
        string PageSql(string sql, string columns, string order);
        IDbConnection Create();
    }
}
