using Acb.Core;
using AutoMapper;
using System.Collections;
using System.Collections.Generic;

namespace Acb.AutoMapper
{
    public static class AutoMapperExtensions
    {

        public static T MapTo<T>(this object source)
        {
            if (source == null)
                return default(T);
            var cfg = new MapperConfiguration(config =>
            {
                config.CreateMap(source.GetType(), typeof(T));
                config.CreateMissingTypeMaps = true;
            });
            return cfg.CreateMapper().Map<T>(source);
        }

        public static List<T> MapTo<T>(this IEnumerable source)
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
            });
            var mapper = cfg.CreateMapper();
            return mapper.Map<List<T>>(source);
        }

        public static PagedList<T> MapPagedList<T, TSource>(this PagedList<TSource> pagedList)
        {
            if (pagedList == null)
                return new PagedList<T>(new List<T>(), 0);
            var list = pagedList.List.MapTo<T>();
            return new PagedList<T>(list, pagedList.Index, pagedList.Size, pagedList.Total);
        }
    }
}
