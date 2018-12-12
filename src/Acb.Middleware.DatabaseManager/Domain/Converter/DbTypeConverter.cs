using Acb.Core;
using Acb.Core.Helper;
using Acb.Middleware.DatabaseManager.Domain.Enums;
using System.IO;
using System.Linq;

namespace Acb.Middleware.DatabaseManager.Domain.Converter
{
    public class DbTypeConverter
    {
        private readonly DbTypeMap _typeMap;
        private DbTypeConverter()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/config/DbTypeMaps.xml");
            _typeMap = XmlHelper.XmlDeserialize<DbTypeMap>(path);
        }

        public static DbTypeConverter Instance =>
            Singleton<DbTypeConverter>.Instance ?? (Singleton<DbTypeConverter>.Instance = new DbTypeConverter());

        /// <summary> 数据库类型转为语言类型 </summary>
        /// <param name="dbProvider"></param>
        /// <param name="lang"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public string LanguageType(DbProvider dbProvider, Language lang, string dbType)
        {
            var databaseMap = _typeMap.Databases.FirstOrDefault(m => m.DbProvider == dbProvider && m.Language == lang);
            if (databaseMap == null)
            {
            }
            var dbTypeMap = databaseMap?.DbTypes?.FirstOrDefault(m => m.Name == dbType);

            if (dbTypeMap == null)
            {
            }

            return dbTypeMap?.To;
        }

        /// <summary> 语言类型转为数据库类型 </summary>
        /// <param name="dbProvider"></param>
        /// <param name="lang"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public string DbType(DbProvider dbProvider, Language lang, string languageType)
        {
            var databaseMap = _typeMap.Databases.FirstOrDefault(m => m.DbProvider == dbProvider && m.Language == lang);
            if (databaseMap == null)
            {
            }
            var dbTypeMap = databaseMap?.DbTypes?.FirstOrDefault(m => m.To == languageType);

            if (dbTypeMap == null)
            {
            }
            return dbTypeMap?.To;
        }
    }
}
