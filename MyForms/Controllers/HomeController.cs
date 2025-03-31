using Microsoft.AspNetCore.Mvc;

namespace MyForms.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
