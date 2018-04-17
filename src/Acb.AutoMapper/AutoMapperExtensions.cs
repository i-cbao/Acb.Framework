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

    public static class AutoMapperExtensions
    {
        public static T MapTo<T>(this object source, MapperType mapperType = MapperType.Normal)
        {
            if (source == null)
                return default(T);
            var cfg = new MapperConfiguration(config =>
            {
                config.CreateMap(source.GetType(), typeof(T));
                config.CreateMissingTypeMaps = true;
                config.ValidateInlineMaps = false;
                if (mapperType == MapperType.FromUrl)
                    config.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                else if (mapperType == MapperType.ToUrl)
                    config.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
            });
            return cfg.CreateMapper().Map<T>(source);
        }

        public static List<T> MapTo<T>(this IEnumerable source, MapperType mapperType = MapperType.Normal)
        {
            if (source == null)
                return new List<T>();
            var cfg = new MapperConfiguration(config =>
            {
                foreach (var item in source)
                {
                    config.CreateMap(item.GetType(), typeof(T));
                    break;
                }

                config.CreateMissingTypeMaps = true;
                config.ValidateInlineMaps = false;
                if (mapperType == MapperType.FromUrl)
                    config.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                else if (mapperType == MapperType.ToUrl)
                    config.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
            });
            var mapper = cfg.CreateMapper();
            return mapper.Map<List<T>>(source);
        }

        public static PagedList<T> MapPagedList<T, TSource>(this PagedList<TSource> pagedList, MapperType mapperType = MapperType.Normal)
        {
            if (pagedList == null)
                return new PagedList<T>(new List<T>(), 0);
            var list = pagedList.List.MapTo<T>(mapperType);
            return new PagedList<T>(list, pagedList.Index, pagedList.Size, pagedList.Total);
        }
    }
}
