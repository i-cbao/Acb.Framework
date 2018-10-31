using System.Threading.Tasks;

namespace Acb.Framework.Tests.Service
{
    public interface IHelloService
    {
        string Hello(string name);
        Task<string> HelloAsync(string name);
    }

    public class HelloService : IHelloService
    {
        public string Hello(string name)
        {
            return $"Hello {name}";
        }

        public async Task<string> HelloAsync(string name)
        {
            //Thread.Sleep(RandomHelper.Random().Next(10) * 1000);
            return await Task.FromResult($"Hello {name}");
        }
    }
}
