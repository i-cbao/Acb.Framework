using Acb.Spear.Contracts.Dtos;
using Acb.Spear.Domain;
using Acb.Spear.Filters;

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

        protected ProjectDto Project
        {
            get
            {
                if (_project != null)
                    return _project;
                return _project = HttpContext.GetProject();
            }
        }
    }
}
