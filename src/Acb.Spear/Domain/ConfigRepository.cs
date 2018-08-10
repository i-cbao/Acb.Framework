using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Timing;
using Acb.Dapper;
using Acb.Dapper.Adapters;
using Acb.Dapper.Domain;
using Acb.Spear.Domain.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Acb.Spear.Domain.Enums;
using IdentityHelper = Acb.Core.Helper.IdentityHelper;

namespace Acb.Spear.Domain
{
    /// <summary> 配置中心仓储类 </summary>
    public class ConfigRepository : DapperRepository<TConfig>
    {
        /// <summary> 添加项目 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> InsertProject(TConfigProject model)
        {
            model.Id = IdentityHelper.Guid32;
            using (var conn = GetConnection(threadCache: false))
            {
                if (string.IsNullOrWhiteSpace(model.Code))
                {
                    var count = await conn.CountWhereAsync<TConfigProject>() + 1;
                    model.Code = $"p{count.ToString().PadLeft(3, '0')}";
                }
                if (await conn.ExistsAsync<TConfigProject>("Code", model.Code))
                    throw new BusiException("项目编码已存在");

                await CheckProjectAccount(conn, model.Code, model.Account);
                return await conn.InsertAsync(model);
            }
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
            using (var conn = GetConnection(threadCache: false))
            {
                await CheckProjectAccount(conn, model.Code, model.Account);
                return await conn.UpdateAsync(model,
                    new[]
                    {
                        nameof(TConfigProject.Name), nameof(TConfigProject.Security),
                        nameof(TConfigProject.Account), nameof(TConfigProject.Password), nameof(TConfigProject.Desc)
                    });
            }
        }

        /// <summary> 检验账号是否存在 </summary>
        /// <param name="conn"></param>
        /// <param name="code"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task CheckProjectAccount(IDbConnection conn, string code, string account)
        {
            var exist = await conn.ExistsWhereAsync<TConfigProject>("[Account]=@account AND [Code]<>@code",
                new { account, code });
            if (exist)
                throw new BusiException("登录账号已存在");
        }

        /// <summary> 查询项目 </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<TConfigProject> QueryProject(string code)
        {
            using (var conn = GetConnection(threadCache: false))
            {
                return await conn.QueryByIdAsync<TConfigProject>(code, "Code");
            }
        }

        /// <summary> 查询项目所有配置名 </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> QueryNames(string code)
        {
            const string sql = "SELECT [Name] FROM [t_config] WHERE [ProjectCode]=@code AND [Status]=0 GROUP BY [Name]";
            using (var conn = GetConnection(threadCache: false))
            {
                return await conn.QueryAsync<string>(conn.FormatSql(sql), new { code });
            }
        }

        /// <summary> 查询配置 </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task<string> QueryConfig(string code, string name, string mode = null)
        {
            const string sql =
                "SELECT [Content] FROM [t_config] WHERE [Status]=0 AND [ProjectCode]=@code AND [Name]=@name AND ([Mode]=@mode OR [Mode] IS NULL) ORDER BY [Mode]";

            using (var conn = GetConnection(threadCache: false))
            {
                return await conn.QueryFirstOrDefaultAsync<string>(conn.FormatSql(sql), new
                {
                    code,
                    name,
                    mode
                });
            }
        }

        /// <summary> 查询配置版本 </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task<string> QueryConfigVersion(string code, string name, string mode = null)
        {
            const string sql =
                "SELECT [Md5] FROM [t_config] WHERE [Status]=0 AND [ProjectCode]=@code AND [Name]=@name AND ([Mode]=@mode OR [Mode] IS NULL) ORDER BY [Mode]";

            using (var conn = GetConnection(threadCache: false))
            {
                return await conn.QueryFirstOrDefaultAsync<string>(conn.FormatSql(sql), new
                {
                    code,
                    name,
                    mode
                });
            }
        }

        /// <summary> 查询配置历史版本 </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<PagedList<TConfig>> QueryHistory(string code, string name, string mode = null, int page = 1, int size = 10)
        {
            const string sql =
                "SELECT * FROM [t_config] WHERE [Status]=1 AND [ProjectCode]=@code AND [Name]=@name AND [Mode]=@mode ORDER BY [Timestamp] DESC";
            using (var conn = GetConnection(threadCache: false))
            {
                return await conn.PagedListAsync<TConfig>(sql, page, size, new { code, name, mode });
            }
        }

        /// <summary> 还原历史版本 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TConfig> RecoveryHistory(string id)
        {
            //更新之前版本为历史版本
            const string updateSql = "UPDATE [t_config] SET [Status]=1 WHERE [ProjectCode]=@code AND [Name]=@name AND [Mode]=@mode AND [Status]=0";
            return await Transaction((conn, trans) =>
            {
                var history = conn.QueryById<TConfig>(id);
                if (history == null || history.Status != (byte)ConfigStatus.History)
                    throw new BusiException("历史版本不存在");

                var count = conn.Execute(conn.FormatSql(updateSql), new
                {
                    code = history.ProjectCode,
                    name = history.Name,
                    mode = history.Mode
                }, trans);
                //更新状态
                history.Status = (byte)ConfigStatus.Normal;
                count += conn.Update(history, new[] { nameof(TConfig.Status) }, trans);
                return Task.FromResult(count > 0 ? history : null);
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
            const string updateSql = "UPDATE [t_config] SET [Status]=1 WHERE [ProjectCode]=@code AND [Name]=@name AND [Mode]=@mode AND [Status]=0";
            return Transaction((conn, trans) =>
            {
                var count = conn.Execute(conn.FormatSql(updateSql), new
                {
                    code = model.ProjectCode,
                    name = model.Name,
                    mode = model.Mode
                }, trans);
                count += conn.Insert(model, trans: trans);
                return count;
            });
        }

        /// <summary> 项目登录 </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<TConfigProject> ProjectLogin(string account, string password)
        {
            using (var conn = GetConnection(threadCache: false))
            {
                var model = await conn.QueryByIdAsync<TConfigProject>(account, "Account");
                if (model == null)
                    throw new BusiException("登录账号不存在");
                if (model.Password != password.Md5())
                    throw new BusiException("登录密码不正确");
                return model;
            }
        }
    }
}
