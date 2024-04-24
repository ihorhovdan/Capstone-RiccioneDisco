using Microsoft.AspNetCore.Mvc;

namespace RiccioneDisco.Controllers
{
    public class AlberghiController : Controller
    {
        public IActionResult Ardea()
        {
            return View();
        }

        public IActionResult LasVegas()
        {
            return View();
        }

        public IActionResult Modenese()
        {
            return View();
        }
    }
}
