using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acb.Framework.Tests
{
    public partial class DapperTest
    {
        [TestMethod]
        public async Task AsyncTest()
        {
            var areas = await _demoService.Areas("110000");
            var result = await _demoService.Update();
            Print(areas);
            Print(result);
        }
    }
}
