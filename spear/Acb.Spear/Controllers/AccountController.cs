using Acb.Core;
using Acb.Spear.Domain;
using Acb.Spear.ViewModels;
using Acb.Spear.ViewModels.Account;
using Acb.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Acb.Spear.Business.Domain.Repositories;
using Acb.Spear.Contracts.Dtos;

namespace Acb.Spear.Controllers
{
    [Route("api/account")]
    public class AccountController : DController
    {
        private readonly AccountRepository _repository;
        private readonly ProjectRepository _projectRepository;

        public AccountController(AccountRepository repository, ProjectRepository projectRepository)
        {
            _repository = repository;
            _projectRepository = projectRepository;
        }

        public Task<DResult> Create([FromBody] VAccountInput input)
        {
            return Task.FromResult(Success);
        }

        /// <summary> 项目登录 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("login"), AllowAnonymous]
        public async Task<DResult<string>> Login([FromBody]VConfigLoginInput input)
        {
            var model = await _repository.LoginAsync(input.Account, input.Password);
            var client = new SpearTicket();
            if (model.ProjectId.HasValue)
            {
                var project = await _projectRepository.QueryByIdAsync(model.ProjectId.Value);
                if (project != null)
                {
                    client.Code = project.Code;
                }
            }
            var ticket = client.Ticket();
            return DResult.Succ(ticket);
        }

        [HttpGet("")]
        public Task<DResult<AccountDto>> Load()
        {
            return Task.FromResult(Succ(new AccountDto
            {
                Id = Ticket.Id,
                Nick = Ticket.Nick,
                Avatar = Ticket.Avatar
            }));
        }
    }
}
