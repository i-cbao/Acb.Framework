using System;

namespace Acb.Backgrounder
{
    public interface IWorkItem
    {
        long Id { get; set; }
        DateTime Started { get; set; }
        DateTime? Completed { get; set; }
    }
}
