﻿namespace Acb.Core.Domain.Entities
{
    public interface IHaveTenant<T>
    {
        T TenantId { get; set; }
    }
}
