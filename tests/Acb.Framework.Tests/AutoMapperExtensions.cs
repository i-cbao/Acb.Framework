using AutoMapper;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Acb.Framework.Tests
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        ///  类型映射
        /// </summary>
        public static T MapTo<T>(this object obj)
        {
            if (obj == null) return default(T);
            var cfg = new MapperConfiguration(config =>
            {
                config.CreateMap(obj.GetType(), typeof(T));
            });
            return cfg.CreateMapper().Map<T>(obj);
        }

        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TDestination>(this IEnumerable source)
        {
            var cfg = new MapperConfiguration(config =>
            {
                foreach (var first in source)
                {
                    var type = first.GetType();
                    config.CreateMap(type, typeof(TDestination));
                    break;
                }
            });

            return cfg.CreateMapper().Map<List<TDestination>>(source);
        }

        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source)
        {

            var cfg = new MapperConfiguration(config =>
            {
                config.CreateMap<TSource, TDestination>();
            });
            return cfg.CreateMapper().Map<List<TDestination>>(source);
        }

        /// <summary>
        /// 类型映射
        /// </summary>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
                    where TSource : class
                    where TDestination : class
        {
            if (source == null)
                return destination;
            var cfg = new MapperConfiguration(config =>
            {
                config.CreateMap<TSource, TDestination>();
            });
            return cfg.CreateMapper().Map(source, destination);
        }
        /// <summary>
        /// DataReader映射
        /// </summary>
        public static IEnumerable<T> DataReaderMapTo<T>(this IDataReader reader)
        {
            var cfg = new MapperConfiguration(config =>
            {
                config.CreateMap<IDataReader, IEnumerable<T>>();
            });
            return cfg.CreateMapper().Map<IDataReader, IEnumerable<T>>(reader);
        }
    }
}
