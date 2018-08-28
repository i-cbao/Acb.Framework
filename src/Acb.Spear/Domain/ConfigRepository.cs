using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Timing;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Spear.Domain.Entities;
using Acb.Spear.Domain.Enums;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acb.Dapper.Adapters;
using IdentityHelper = Acb.Core.Helper.IdentityHelper;

namespace Acb.Spear.Domain
{
    /// <summary> 配置中心仓储类 </summary>
    public class ConfigRepository : DapperRepository<TConfig>
    {
        public ConfigRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary> 添加项目 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> InsertProject(TConfigProject model)
        {
            model.Id = IdentityHelper.Guid32;
            if (string.IsNullOrWhiteSpace(model.Code))
            {
                var count = await Connection.CountWhereAsync<TConfigProject>() + 1;
                model.Code = $"p{count.ToString().PadLeft(3, '0')}";
            }
            if (await Connection.ExistsAsync<TConfigProject>("Code", model.Code))
                throw new BusiException("项目编码已存在");

            await CheckProjectAccount(model.Code, model.Account);
            return await Connection.InsertAsync(model, trans: Trans);
        }

        /// <summary> 更新项目 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> UpdateProject(TConfigProject model)
        {
            var project = await QueryProject(model.Code);
            if (project == null)
                throw new BusiException("项目不存在");
            model.Id = project.Id;
            await CheckProjectAccount(model.Code, model.Account);
            return await Connection.UpdateAsync(model,
                new[]
                {
                    nameof(TConfigProject.Name), nameof(TConfigProject.Security),
                    nameof(TConfigProject.Account), nameof(TConfigProject.Password), nameof(TConfigProject.Desc)
                }, Trans);
        }

        /// <summary> 检验账号是否存在 </summary>
        /// <param name="code"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task CheckProjectAccount(string code, string account)
        {
            var exist = await Connection.ExistsWhereAsync<TConfigProject>("[Account]=@account AND [Code]<>@code",
                new { account, code });
            if (exist)
                throw new BusiException("登录账号已存在");
        }

        /// <summary> 查询项目 </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<TConfigProject> QueryProject(string code)
        {
            return await Connection.QueryByIdAsync<TConfigProject>(code, "Code");
        }

        /// <summary> 项目登录 </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<TConfigProject> ProjectLogin(string account, string password)
        {
            var model = await Connection.QueryByIdAsync<TConfigProject>(account, "Account");
            if (model == null)
                throw new BusiException("登录账号不存在");
            if (model.Password != password.Md5())
                throw new BusiException("登录密码不正确");
            return model;
        }

        /// <summary> 查询项目所有配置名 </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> QueryNames(string code)
        {
            const string sql = "SELECT [Name] FROM [t_config] WHERE [ProjectCode]=@code AND [Status]=0 GROUP BY [Name]";

            return await Connection.QueryAsync<string>(Connection.FormatSql(sql), new { code });
        }

        /// <summary> 查询配置 </summary>
        /// <param name="code"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task<string> QueryConfig(string code, string module, string env = null)
        {
            const string sql =
                "SELECT [Content] FROM [t_config] WHERE [Status]=0 AND [ProjectCode]=@code AND [Name]=@name AND ([Mode]=@mode OR [Mode] IS NULL) ORDER BY [Mode]";

            return await Connection.QueryFirstOrDefaultAsync<string>(Connection.FormatSql(sql), new
            {
                code,
                name = module,
                mode = env
            });
        }

        /// <summary> 查询配置版本 </summary>
        /// <param name="code"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task<string> QueryConfigVersion(string code, string module, string env = null)
        {
            const string sql =
                "SELECT [Md5] FROM [t_config] WHERE [Status]=0 AND [ProjectCode]=@code AND [Name]=@name AND ([Mode]=@mode OR [Mode] IS NULL) ORDER BY [Mode]";

            return await Connection.QueryFirstOrDefaultAsync<string>(Connection.FormatSql(sql), new
            {
                code,
                name = module,
                mode = env
            });
        }

        /// <summary> 查询配置历史版本 </summary>
        /// <param name="code"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<PagedList<TConfig>> QueryHistory(string code, string module, string env = null, int page = 1, int size = 10)
        {
            const string sql =
                "SELECT * FROM [t_config] WHERE [Status]=1 AND [ProjectCode]=@code AND [Name]=@name AND [Mode]=@mode ORDER BY [Timestamp] DESC";

            return await Connection.PagedListAsync<TConfig>(sql, page, size, new { code, name = module, mode = env });
        }

        /// <summary> 还原历史版本 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TConfig> RecoveryHistory(string id)
        {
            //更新之前版本为历史版本
            const string updateSql = "UPDATE [t_config] SET [Status]=1 WHERE [ProjectCode]=@code AND [Name]=@name AND [Mode]=@mode AND [Status]=0";
            return await Transaction(async () =>
            {
                var history = await Connection.QueryByIdAsync<TConfig>(id);
                if (history == null || history.Status != (byte)ConfigStatus.History)
                    throw new BusiException("历史版本不存在");

                var count = await Connection.ExecuteAsync(Connection.FormatSql(updateSql), new
                {
                    code = history.ProjectCode,
                    name = history.Name,
                    mode = history.Mode
                }, Trans);
                //更新状态
                history.Status = (byte)ConfigStatus.Normal;
                count += await Connection.UpdateAsync(history, new[] { nameof(TConfig.Status) }, Trans);
                return count > 0 ? history : null;
            });
        }

        /// <summary> 保存配置 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> SaveConfig(TConfig model)
        {
            var project = await QueryProject(model.ProjectCode);
            if (project == null)
                throw new BusiException("项目不存在");
            model.Id = IdentityHelper.Guid32;
            model.Timestamp = Clock.Now;
            model.Md5 = model.Content.Md5();
            var version = await QueryConfigVersion(model.ProjectCode, model.Name, model.Mode);
            if (version != null && version == model.Md5)
                throw new BusiException("配置未更改");
            //更新之前版本为历史版本
            return await Transaction(async () =>
            {
                var count = await DeleteConfig(model.ProjectCode, model.Name, model.Mode);
                count += await Connection.InsertAsync(model, trans: Trans);
                return count;
            });
        }

        /// <summary> 删除配置 </summary>
        /// <param name="code"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task<int> DeleteConfig(string code, string module, string env)
        {
            //更新之前版本为历史版本
            const string updateSql =
                "UPDATE [t_config] SET [Status]=1 WHERE [ProjectCode]=@code AND [Name]=@name AND [Mode]=@mode AND [Status]=0";
            return await Connection.ExecuteAsync(Connection.FormatSql(updateSql), new { code, name = module, mode = env },
                Trans);
        }
    }
}
