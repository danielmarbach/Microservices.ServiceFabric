using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using Contracts;

namespace ChocolateOrder.Front.Controllers
{
    public class HomeController : Controller
    {
        private IMessageSession messageSession;

        public HomeController(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Dark()
        {
            await messageSession.Send(new OrderChocolate { ChocolateType = "Dark" });
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Brown()
        {
            await messageSession.Send(new OrderChocolate { ChocolateType = "Brown" });
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> White()
        {
            await messageSession.Send(new OrderChocolate { ChocolateType = "White" });
            return View("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
