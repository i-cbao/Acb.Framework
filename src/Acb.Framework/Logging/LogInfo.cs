
using System;
using System.Text;

namespace Acb.Framework.Logging
{
    [Serializable]
    public class LogInfo
    {
        public string SiteName { get; set; }
        public string Method { get; set; }
        public string File { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{SiteName}:{File}");
            if (!string.IsNullOrWhiteSpace(Method))
                sb.AppendLine($"method:{Method}");
            if (!string.IsNullOrWhiteSpace(Message))
                sb.AppendLine($"message:{Message}");
            if (!string.IsNullOrWhiteSpace(Detail))
                sb.AppendLine($"detail:{Detail}");
            return sb.ToString();
        }
    }
}
