using Microsoft.AspNetCore.Mvc;
using RiccioneDisco.Models;

namespace RiccioneDisco.Controllers
{
    public class ChatBotController : Controller
    {
        private ChatBotService _chatBotService = new ChatBotService();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetBotResponse(string message)
        {
            var response = _chatBotService.GetResponse(message);
            return Json(response);
        }
    }
}
