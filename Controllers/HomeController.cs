using Microsoft.AspNetCore.Mvc;

namespace Praktikabitdi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
