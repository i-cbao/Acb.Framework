﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Acb.Middleware.DatabaseManager.Domain.Enums;
using Acb.Middleware.DatabaseManager.Domain.Models;

namespace Acb.Middleware.DatabaseManager.Domain
{
    public interface IDatabaseService
    {
        string DbName { get; }
        DbProvider Provider { get; }
        Task<IEnumerable<Table>> GetTablesAsync();
    }
}
