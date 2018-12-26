using Acb.Core;
using Acb.Core.Helper;
using Acb.Core.Logging;
using Acb.Middleware.DatabaseManager.Domain.Enums;
using System.IO;
using System.Linq;

namespace Acb.Middleware.DatabaseManager.Domain.Converter
{
    public class DbTypeConverter
    {
        private readonly DbTypeMap _typeMap;
        private readonly ILogger _logger;
        private DbTypeConverter()
        {
            _logger = LogManager.Logger<DbTypeConverter>();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/config/DbTypeMaps.xml");
            _typeMap = XmlHelper.XmlDeserialize<DbTypeMap>(path);
        }

        public static DbTypeConverter Instance =>
            Singleton<DbTypeConverter>.Instance ?? (Singleton<DbTypeConverter>.Instance = new DbTypeConverter());

        /// <summary> 数据库类型转为语言类型 </summary>
        /// <param name="dbProvider">数据库驱动</param>
        /// <param name="lang">语言</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="isNullable">是否可为空</param>
        /// <returns></returns>
        public string LanguageType(DbProvider dbProvider, Language lang, string dbType, bool isNullable = false)
        {
            var dbTypeMap = new DbTypeMapLange
            {
                Name = dbType,
                To = dbType
            };

            var databaseMap = _typeMap.Databases.FirstOrDefault(m => m.DbProvider == dbProvider && m.Language == lang);
            if (databaseMap == null)
            {
                _logger.Warn($"没有找到语言对应的映射关系:{dbProvider}->{lang}");
            }
            else
            {
                var map = databaseMap.DbTypes?.FirstOrDefault(m => m.Name == dbType);

                if (map == null)
                {
                    _logger.Warn($"没有找到语言对应的数据类型:{dbProvider}->{lang},{dbType}");
                }
                else
                {
                    dbTypeMap = map;
                }
            }

            if (lang == Language.CSharp)
            {
                return dbTypeMap.To != "string" && isNullable ? $"{dbTypeMap.To}?" : dbTypeMap.To;
            }
            //:todo 其他语言
            return dbTypeMap.To;
        }

        /// <summary> 语言类型转为数据库类型 </summary>
        /// <param name="dbProvider"></param>
        /// <param name="lang"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public string DbType(DbProvider dbProvider, Language lang, string languageType)
        {
            var dbTypeMap = new DbTypeMapLange
            {
                Name = languageType,
                To = languageType
            };
            var databaseMap = _typeMap.Databases.FirstOrDefault(m => m.DbProvider == dbProvider && m.Language == lang);
            if (databaseMap == null)
            {
                _logger.Warn($"没有找到语言对应的映射关系:{dbProvider}->{lang}");
            }
            var map = databaseMap?.DbTypes?.FirstOrDefault(m => m.To == languageType);

            if (map == null)
            {
                _logger.Warn($"没有找到语言类型对应的数据类型:{dbProvider}->{lang},{languageType}");
            }
            else
            {
                dbTypeMap = map;
            }
            return dbTypeMap.To;
        }
    }
}
