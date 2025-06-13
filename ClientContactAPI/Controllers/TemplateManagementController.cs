using Microsoft.AspNetCore.Mvc;

namespace ClientContactAPI.Controllers
{
    public class TemplateManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
