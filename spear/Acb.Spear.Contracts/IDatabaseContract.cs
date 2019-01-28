using Acb.Core;
using Acb.Core.Dependency;
using Acb.Spear.Contracts.Dtos.Database;
using Acb.Spear.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Acb.Spear.Contracts
{
    /// <inheritdoc />
    /// <summary> 数据库相关契约 </summary>
    public interface IDatabaseContract : IDependency
    {
        /// <summary> 添加数据库连接 </summary>
        /// <param name="accountId"></param>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        Task<int> AddAsync(Guid accountId, string name, ProviderType provider, string connectionString);

        /// <summary> 获取数据表 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DatabaseTablesDto> GetAsync(Guid id);

        /// <summary> 数据库列表 </summary>
        /// <param name="accountId"></param>
        /// <param name="type"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PagedList<DatabaseDto>> PagedListAsync(Guid accountId, ProviderType? type = null, int page = 1,
            int size = 10);

        /// <summary> 更新数据库配置 </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        Task<int> SetAsync(Guid id, string name, ProviderType type, string connectionString);

        /// <summary> 删除数据库配置 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> RemoveAsync(Guid id);

        /// <summary> 转换数据类型 </summary>
        /// <param name="dbType"></param>
        /// <param name="provider"></param>
        /// <param name="language"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        string ConvertToLanguageType(string dbType, ProviderType provider, LanguageType language,
            bool isNullable = false);

        /// <summary> 转换数据类型 </summary>
        /// <param name="languageType"></param>
        /// <param name="provider"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        string ConvertToDbType(string languageType, ProviderType provider, LanguageType language);
    }
}
