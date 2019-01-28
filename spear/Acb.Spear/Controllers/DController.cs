using Acb.Core;
using Acb.Core.Extensions;
using Acb.Spear.Contracts.Dtos;
using Acb.Spear.Domain;
using Acb.Spear.Filters;
using System;

namespace Acb.Spear.Controllers
{
    [SpearAuthorize]
    public class DController : WebApi.DController
    {
        private SpearTicket _ticket;

        /// <summary> 登录密钥 </summary>
        protected SpearTicket Ticket
        {
            get
            {
                if (_ticket != null)
                    return _ticket;
                var ticket = HttpContext.GetTicket();
                return _ticket = ticket;
            }
        }

        private string _projectCode;

        /// <summary> 项目编码 </summary>
        protected string ProjectCode
        {
            get
            {
                if (_projectCode != null)
                    return _projectCode;
                return _projectCode = HttpContext.GetProjectCode();
            }
        }

        private ProjectDto _project;

        /// <summary> 项目ID </summary>
        protected Guid ProjectId => Project.Id;

        /// <summary> 项目信息 </summary>
        protected ProjectDto Project
        {
            get
            {
                if (_project != null)
                    return _project;
                return _project = HttpContext.GetProject();
            }
        }

        /// <summary> 转换DResult </summary>
        /// <param name="result"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        protected DResult FromResult<T>(T result, string errorMsg = null)
        where T : struct
        {
            bool success;
            if (typeof(T) == typeof(bool))
            {
                success = result.CastTo<bool>();
            }
            else
            {
                success = result.CastTo(0) > 0;
            }

            return success ? Success : Error(string.IsNullOrWhiteSpace(errorMsg) ? "操作失败，请重试" : errorMsg);
        }
    }
}
