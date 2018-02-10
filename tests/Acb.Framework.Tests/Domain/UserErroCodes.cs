using System.ComponentModel;
using Acb.Core.Exceptions;

namespace Acb.Framework.Tests.Domain
{
    public class UserErroCodes : ErrorCodes
    {
        [Description("用户异常")]
        public const int UserError = 21001;
    }
}
