using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace OrderChocolate
{

    // TODO 1
    public interface IChocolateService : IService
    {
        Task<Result> SayHello();
    }
}