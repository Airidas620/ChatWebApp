using Microsoft.AspNetCore.Mvc;

namespace HermeApp.Web.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult MainPage()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            return Redirect("/Identity/Account/Login");
        }
    }
}
