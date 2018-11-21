using Acb.Core.Exceptions;
using Acb.Demo.Business.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acb.Framework.Tests
{
    [TestClass]
    public class ErrorCodesTest : DTest
    {
        [TestMethod]
        public void CodesTest()
        {
            var codes = typeof(DTest).Codes();
            Print(codes);
            var result = UserErroCodes.UserError.CodeResult<UserErroCodes>();
            Print(result);
        }
    }
}
