using Acb.Core.Extensions;

namespace Acb.WebApi.Test.ViewModels
{
    public class DemoClientTicket : DClientTicket<long>
    {
        public override string GenerateTicket()
        {
            return $"id:{Id},name:{Name},role:{Role},timestamp:{Timestamp.Ticks}".Md5();
        }
    }
}
