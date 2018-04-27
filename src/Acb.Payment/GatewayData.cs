using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Serialize;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;

namespace Acb.Payment
{
    /// <summary> 支付网关数据 </summary>
    public class GatewayData
    {
        private readonly SortedDictionary<string, object> _values;

        /// <summary> 原始数据 </summary>
        public string OriginalData { get; private set; }
        /// <summary> 构造函数 </summary>
        public GatewayData()
        {
            _values = new SortedDictionary<string, object>();
        }

        /// <summary> 构造函数 </summary>
        /// <param name="comparer">排序策略</param>
        public GatewayData(IComparer<string> comparer)
        {
            _values = new SortedDictionary<string, object>(comparer);
        }

        public object this[string key]
        {
            get => _values[key];
            set => _values[key] = value;
        }

        public int Count => _values.Count;

        /// <summary>
        /// 是否存在指定参数名
        /// </summary>
        /// <param name="key">参数名</param>
        /// <returns></returns>
        public bool Exists(string key) => _values.ContainsKey(key);

        /// <summary>
        /// 清空网关数据
        /// </summary>
        public void Clear()
        {
            _values.Clear();
        }

        /// <summary>
        /// 移除指定参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return _values.Remove(key);
        }

        #region 添加参数
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new BusiException("参数名不能为空");
            if (Exists(key))
            {
                _values[key] = value;
            }
            else
            {
                _values.Add(key, value);
            }
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="namingType"></param>
        public void Add(object obj, NamingType? namingType = null)
        {
            var type = obj.GetType();
            var properties = type.GetProperties();
            var fields = type.GetFields();
            if (!namingType.HasValue)
            {
                var typeProp = type.GetCustomAttribute<NamingAttribute>(true);
                if (typeProp != null)
                    namingType = typeProp.NamingType;
            }

            AddMembers(properties);
            AddMembers(fields);

            void AddMembers(MemberInfo[] info)
            {
                foreach (var item in info)
                {
                    string key = item.PropName(namingType);
                    if (string.IsNullOrWhiteSpace(key))
                        continue;

                    object value;

                    switch (item.MemberType)
                    {
                        case MemberTypes.Field:
                            value = ((FieldInfo)item).GetValue(obj);
                            break;
                        case MemberTypes.Property:
                            value = ((PropertyInfo)item).GetValue(obj);
                            break;
                        default:
                            value = null;
                            break;
                    }

                    if (value is null || string.IsNullOrEmpty(value.ToString()))
                    {
                        continue;
                    }
                    Add(key, value);
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public T GetValue<T>(string key, T def = default(T))
        {
            _values.TryGetValue(key, out object value);
            if (value is StringValues values)
                return values.ToString().CastTo(def);
            return value.CastTo(def);
        }

        /// <summary>
        /// 将网关数据转成Xml格式数据
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            return _values.ToXml();
        }

        /// <summary>
        /// 将Xml格式数据转换为网关数据
        /// </summary>
        /// <param name="xml">Xml数据</param>
        /// <returns></returns>
        public void FromXml(string xml)
        {
            try
            {
                _values.FromXml(xml);
            }
            finally
            {
                OriginalData = xml;
            }
        }
        /// <summary>
        /// 将网关数据转换为Url格式数据
        /// </summary>
        /// <param name="encode">是否需要url编码</param>
        /// <returns></returns>
        public string ToUrl(bool encode = true)
        {
            return _values.ToUrl(encode);
        }

        /// <summary>
        /// 将Url格式数据转换为网关数据
        /// </summary>
        /// <param name="url">url数据</param>
        /// <param name="isUrlDecode">是否需要url解码</param>
        /// <returns></returns>
        public void FromUrl(string url, bool isUrlDecode = true)
        {
            try
            {
                _values.FromUrl(url, isUrlDecode);
            }
            finally
            {
                OriginalData = url;
            }
        }

        /// <summary>
        /// 将键值对转换为网关数据
        /// </summary>
        /// <param name="nvc">键值对</param>
        /// <returns></returns>
        public void FromNameValueCollection(NameValueCollection nvc)
        {
            _values.FromNameValueCollection(nvc);
        }

        /// <summary>
        /// 将表单数据转换为网关数据
        /// </summary>
        /// <param name="form">表单</param>
        /// <returns></returns>
        public void FromForm(IFormCollection form)
        {
            Clear();
            var allKeys = form.Keys;

            foreach (var item in allKeys)
            {
                Add(item, form[item].ToString());
            }
        }

        /// <summary>
        /// 将网关数据转换为表单数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public string ToForm(string url)
        {
            return _values.ToForm(url);
        }

        /// <summary>
        /// 将网关数据转成Json格式数据
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return _values.ToJson();
        }

        /// <summary>
        /// 将Json格式数据转成网关数据
        /// </summary>
        /// <param name="json">json数据</param>
        /// <returns></returns>
        public void FromJson(string json)
        {
            try
            {
                _values.FromJson(json);
            }
            finally
            {
                OriginalData = json;
            }
        }

        /// <summary>
        /// 将网关参数转为类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="namingType">字符串策略</param>
        /// <returns></returns>
        public T ToObject<T>(NamingType? namingType = null)
        {
            return _values.ToObject<T>(namingType);
        }

        /// <summary>
        /// 异步将网关参数转为类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="namingType">字符串策略</param>
        /// <returns></returns>
        public async Task<T> ToObjectAsync<T>(NamingType namingType)
        {
            return await Task.Run(() => ToObject<T>(namingType));
        }
    }
}
