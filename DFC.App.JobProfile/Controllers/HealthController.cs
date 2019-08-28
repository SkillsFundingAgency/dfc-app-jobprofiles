using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.Controllers
{
    public class HealthController : Controller
    {
        private readonly ILogger<HealthController> logger;

        public HealthController(ILogger<HealthController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("health/ping")]
        public IActionResult Ping()
        {
            logger.LogInformation($"{nameof(Ping)} has been called");

            return Ok();
        }
    }
}
