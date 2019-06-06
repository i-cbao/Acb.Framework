using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acb.FileServer.Contracts.Services
{
    public class OcrService : IOcrContract
    {
        public Task<IDictionary<string, string>> IdCarDAsync(string base64File)
        {
            throw new System.NotImplementedException();
        }

        public Task<IDictionary<string, string>> DriverLicenseAsync(string base64File)
        {
            throw new System.NotImplementedException();
        }

        public Task<IDictionary<string, string>> DrivingLicenseAsync(string base64File)
        {
            throw new System.NotImplementedException();
        }

        public Task<IDictionary<string, string>> BusiLicenseAsync(string base64File)
        {
            throw new System.NotImplementedException();
        }

        public Task<IDictionary<string, string>> VehicleQcAsync(string base64File)
        {
            throw new System.NotImplementedException();
        }

        public Task<IDictionary<string, string>> VehicleInvoiceAsync(string base64File)
        {
            throw new System.NotImplementedException();
        }
    }
}
