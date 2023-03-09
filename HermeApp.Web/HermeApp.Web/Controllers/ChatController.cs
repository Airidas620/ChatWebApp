using Microsoft.AspNetCore.Mvc;

namespace HermeApp.Web.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult MainPage()
        {
            return View();
        }
    }
}
