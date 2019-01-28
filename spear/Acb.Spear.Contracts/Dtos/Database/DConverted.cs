using System.Text;
using Acb.Core.Domain.Dtos;

namespace Acb.Spear.Contracts.Dtos.Database
{
    public abstract class DConverted : DDto, IConvertedName
    {
        public abstract string Name { get; set; }

        public virtual string ConvertedName
        {
            get
            {
                var arr = Name.Split('_');
                var sb = new StringBuilder();
                foreach (var s in arr)
                {
                    if (string.IsNullOrWhiteSpace(s)) continue;
                    sb.Append(s.Substring(0, 1).ToUpper());
                    if (s.Length > 1)
                        sb.Append(s.Substring(1));
                }
                return sb.ToString();
            }
        }

        public bool IsConverted => Name != ConvertedName;
    }
}
