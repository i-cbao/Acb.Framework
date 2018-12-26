using System;

namespace Acb.Core.Config.Xml
{
    /// <summary>
    /// 配置文件名属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FileNameAttribute : Attribute
    {
        public string Name { get; set; }

        public FileNameAttribute(string name)
        {
            Name = name;
        }
    }
}
