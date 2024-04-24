using Microsoft.AspNetCore.Mvc;

namespace RiccioneDisco.Controllers
{
    public class ChiSiamoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
