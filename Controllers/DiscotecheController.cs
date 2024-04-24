using Microsoft.AspNetCore.Mvc;

namespace RiccioneDisco.Controllers
{
    public class DiscotecheController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
