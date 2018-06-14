using System.Threading.Tasks;

namespace Acb.Backgrounder
{
    public interface IJobHost
    {
        void DoWork(Task worker);
    }
}
