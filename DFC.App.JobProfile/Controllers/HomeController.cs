using DFC.App.JobProfile.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DFC.App.JobProfile.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logService;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logService = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            logService.LogInformation($"{nameof(Error)} has been called");

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
