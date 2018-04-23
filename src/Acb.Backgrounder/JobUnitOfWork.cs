using System;

namespace Acb.Backgrounder
{
    public class JobUnitOfWork
    {
        private readonly IWorkItemRepository _repository;
        private readonly long _workItemId;

        public JobUnitOfWork(IWorkItemRepository repository, long workItemId)
        {
            _workItemId = workItemId;
            _repository = repository;
        }

        /// <summary> 任务完成一次 </summary>
        public void Complete()
        {
            _repository.SetWorkItemCompleted(_workItemId);
        }

        /// <summary> 任务失败 </summary>
        /// <param name="exception"></param>
        public void Fail(Exception exception)
        {
            _repository.SetWorkItemFailed(_workItemId, exception);
        }
    }
}
