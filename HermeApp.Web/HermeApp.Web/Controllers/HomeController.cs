using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HermeApp.Web.Models;
using HermeApp.Web.AdditionalClasses;

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
            return View(userConnectionTracker);
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
