using Microsoft.AspNetCore.Mvc;

namespace MVCWebApiStarter.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
