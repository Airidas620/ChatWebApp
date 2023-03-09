using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HermeApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using HermeApp.Web.Areas.Identity.Data;

namespace HermeApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IUserConnectionTracker userConnectionTracker;

        public HomeController(ILogger<HomeController> logger, IUserConnectionTracker userConnectionTracker)
        {
            _logger = logger;
            this.userConnectionTracker = userConnectionTracker;
        }

        public IActionResult Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MainPage", "Chat");
            }
            return Redirect("/Identity/Account/Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
