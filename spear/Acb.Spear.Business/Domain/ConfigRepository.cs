using Acb.Core;
using Acb.Core.Data;
using Acb.Core.Domain;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Spear.Business.Domain.Entities;
using Acb.Spear.Domain.Enums;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.Spear.Business.Domain
{
    /// <summary> 配置中心仓储类 </summary>
    public class ConfigRepository : DapperRepository<TConfig>
    {
        public ConfigRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary> 查询项目所有配置名 </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> QueryNamesAsync(Guid projectId)
        {
            const string sql = "SELECT [Name] FROM [t_config] WHERE [ProjectId]=@projectId AND [Status]=0 GROUP BY [Name]";

            return await Connection.QueryAsync<string>(Connection.FormatSql(sql), new { projectId });
        }

        /// <summary> 查询配置 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task<string> QueryByModuleAsync(Guid projectId, string module, string env = null)
        {
            const string sql =
                "SELECT [Content] FROM [t_config] WHERE [Status]=0 AND [ProjectId]=@projectId AND [Name]=@name AND ([Mode]=@mode OR [Mode] IS NULL) ORDER BY [Mode]";

            return await Connection.QueryFirstOrDefaultAsync<string>(Connection.FormatSql(sql), new
            {
                projectId,
                name = module,
                mode = env
            });
        }

        /// <summary> 查询配置版本 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task<string> QueryVersionAsync(Guid projectId, string module, string env = null)
        {
            const string sql =
                "SELECT [Md5] FROM [t_config] WHERE [Status]=0 AND [ProjectId]=@projectId AND [Name]=@name AND ([Mode]=@mode OR [Mode] IS NULL) ORDER BY [Mode]";

            return await Connection.QueryFirstOrDefaultAsync<string>(Connection.FormatSql(sql), new
            {
                projectId,
                name = module,
                mode = env
            });
        }

        /// <summary> 查询配置历史版本 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<PagedList<TConfig>> QueryHistoryAsync(Guid projectId, string module, string env = null, int page = 1, int size = 10)
        {
            const string sql =
                "SELECT * FROM [t_config] WHERE [Status]=1 AND [ProjectId]=@projectId AND [Name]=@name AND [Mode]=@mode ORDER BY [Timestamp] DESC";

            return await Connection.PagedListAsync<TConfig>(sql, page, size, new { projectId, name = module, mode = env });
        }

        /// <summary> 还原历史版本 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TConfig> RecoveryAsync(Guid id)
        {
            //更新之前版本为历史版本
            const string updateSql = "UPDATE [t_config] SET [Status]=1 WHERE [ProjectId]=@projectId AND [Name]=@name AND [Mode]=@mode AND [Status]=0";
            return await Transaction(async () =>
            {
                var history = await Connection.QueryByIdAsync<TConfig>(id);
                if (history == null || history.Status != (byte)ConfigStatus.History)
                    throw new BusiException("历史版本不存在");

                var count = await Connection.ExecuteAsync(Connection.FormatSql(updateSql), new
                {
                    projectId = history.ProjectId,
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
        public async Task<int> UpdateAsync(TConfig model)
        {
            model.Id = IdentityHelper.NewSequentialGuid();
            model.Timestamp = Clock.Now;
            model.Md5 = model.Content.Md5();
            var version = await QueryVersionAsync(model.ProjectId, model.Name, model.Mode);
            if (version != null && version == model.Md5)
                throw new BusiException("配置未更改");
            //更新之前版本为历史版本
            return await Transaction(async () =>
            {
                var count = await DeleteByModuleAsync(model.ProjectId, model.Name, model.Mode);
                count += await Connection.InsertAsync(model, trans: Trans);
                return count;
            });
        }

        /// <summary> 删除配置 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task<int> DeleteByModuleAsync(Guid projectId, string module, string env)
        {
            //更新之前版本为历史版本
            SQL updateSql =
                "UPDATE [t_config] SET [Status]=1 WHERE [ProjectId]=@projectId AND [Name]=@name AND [Status]=0";
            if (string.IsNullOrWhiteSpace(env))
            {
                updateSql += "AND [Mode] IS NULL";
            }
            else
            {
                updateSql += "AND [Mode]=@mode";
            }

            var sql = Connection.FormatSql(updateSql.ToString());
            return await Connection.ExecuteAsync(sql, new { projectId, name = module, mode = env },
                Trans);
        }
    }
}
