using System;

namespace Acb.Core.Serialize
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class NamingAttribute : Attribute
    {

        public string Name { get; }
        public bool Ignore { get; }

        public NamingType NamingType { get; }

        public NamingAttribute(NamingType namingType)
        {
            NamingType = namingType;
        }

        public NamingAttribute(string name)
        {
            Name = name;
        }
        public NamingAttribute(bool ignore)
        {
            Ignore = ignore;
        }
    }
}
