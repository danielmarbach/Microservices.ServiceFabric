using System;
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
            // TODO 1
            await OrderChocolate("Dark");
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Brown()
        {
            await OrderChocolate("Brown");
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> White()
        {
            await OrderChocolate("White");
            return View("Index");
        }

        private async Task OrderChocolate(string chocolateType)
        {
            await messageSession.Send(new OrderChocolate
            {
                OrderId = $"{chocolateType};{Guid.NewGuid()}", // combined order id
                ChocolateType = chocolateType
            });
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
