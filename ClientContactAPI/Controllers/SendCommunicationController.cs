using Microsoft.AspNetCore.Mvc;

namespace ClientContactAPI.Controllers
{
    public class SendCommunicationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
