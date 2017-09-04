using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace OrderChocolate
{
    public interface IChocolateService : IService
    {
        Task<Result> SayHello();
    }
}