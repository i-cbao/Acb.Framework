using Acb.Core;
using AutoMapper;
using System.Collections;
using System.Collections.Generic;

namespace Acb.AutoMapper
{
    public enum MapperType
    {
        /// <summary> 普通 </summary>
        Normal,
        /// <summary> 从url命名到驼峰命名 </summary>
        FromUrl,
        /// <summary> 从驼峰命名url命名 </summary>
        ToUrl
    }

    /// <summary> AutoMapper扩展 </summary>
    public static class AutoMapperExtensions
    {
        /// <summary> 映射实体 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="mapperType"></param>
        /// <returns></returns>
        public static T MapTo<T>(this object source, MapperType mapperType = MapperType.Normal)
        {
            if (source == null)
                return default(T);
            var cfg = new MapperConfiguration(config =>
            {
                if (source is IEnumerable listSource)
                {
                    foreach (var item in listSource)
                    {
                        config.CreateMap(item.GetType(), typeof(T));
                        break;
                    }
                }
                else
                {
                    config.CreateMap(source.GetType(), typeof(T));
                }

                config.CreateMissingTypeMaps = true;
                config.ValidateInlineMaps = false;
                switch (mapperType)
                {
                    case MapperType.FromUrl:
                        config.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                        break;
                    case MapperType.ToUrl:
                        config.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
                        break;
                }
            });
            var mapper = cfg.CreateMapper();
            return mapper.Map<T>(source);
        }

        /// <summary> 映射实体 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="mapperType"></param>
        /// <returns></returns>
        public static List<T> MapTo<T>(this IEnumerable source, MapperType mapperType = MapperType.Normal)
        {
            return ((object)source).MapTo<List<T>>(mapperType);
        }

        /// <summary> 映射成分页类型 </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="pagedList"></param>
        /// <param name="mapperType"></param>
        /// <returns></returns>
        public static PagedList<T> MapPagedList<T, TSource>(this PagedList<TSource> pagedList, MapperType mapperType = MapperType.Normal)
        {
            if (pagedList == null)
                return new PagedList<T>(new List<T>(), 0);
            var list = pagedList.List.MapTo<T>(mapperType);
            return new PagedList<T>(list, pagedList.Index, pagedList.Size, pagedList.Total);
        }
    }
}
