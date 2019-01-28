using Acb.Core.Domain;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Acb.Demo.Business.Domain.Entities;
using System.Threading.Tasks;
using Acb.Core.Exceptions;

namespace Acb.Demo.Business.Domain
{
    public class AnotherAreaRepository : DapperRepository<TAreas>
    {
        public AnotherAreaRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<int> UpdateParent()
        {
            return await Transaction(async () =>
            {
                var count = await Connection.UpdateAsync(new TAreas { Id = "110000", ParentCode = "0" },
                    new[] { nameof(TAreas.ParentCode) }, Trans);
                //throw new BusiException("ex test");
                return count;
            });
        }


    }
}
