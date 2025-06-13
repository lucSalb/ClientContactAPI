using Microsoft.AspNetCore.Mvc;

namespace ClientContactAPI.Controllers
{
    public class CustomerManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
