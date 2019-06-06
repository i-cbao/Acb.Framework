using Acb.Core;
using Acb.Core.Dependency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.FileServer.Contracts
{
    public interface IOcrContract : IMicroService, IDependency
    {
        /// <summary> 身份证 </summary>
        /// <returns></returns>
        Task<IDictionary<string, string>> IdCarDAsync(string base64File);

        /// <summary> 驾驶证 </summary>
        /// <returns></returns>
        Task<IDictionary<string, string>> DriverLicenseAsync(string base64File);

        /// <summary> 行驶证 </summary>
        /// <returns></returns>
        Task<IDictionary<string, string>> DrivingLicenseAsync(string base64File);

        /// <summary> 营业执照 </summary>
        /// <returns></returns>
        Task<IDictionary<string, string>> BusiLicenseAsync(string base64File);

        /// <summary> 车辆合格证 </summary>
        /// <returns></returns>
        Task<IDictionary<string, string>> VehicleQcAsync(string base64File);

        /// <summary> 机动车发票 </summary>
        /// <returns></returns>
        Task<IDictionary<string, string>> VehicleInvoiceAsync(string base64File);
    }
}
