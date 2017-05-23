using Microsoft.AspNetCore.Mvc;
using NServiceBus;

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

        public IActionResult Error()
        {
            return View();
        }
    }
}
