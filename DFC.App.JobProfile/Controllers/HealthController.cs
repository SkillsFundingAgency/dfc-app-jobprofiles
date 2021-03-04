using DFC.App.JobProfile.Data.Providers;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.ViewSupport.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    public class HealthController : Controller
    {
        private readonly ILogger<HealthController> _logger;
        private readonly IProvideJobProfiles _jobProfiles;
        private readonly string resourceName = typeof(Program).Namespace!;

        public HealthController(
            ILogger<HealthController> logService,
            IProvideJobProfiles jobProfileService)
        {
            _logger = logService;
            _jobProfiles = jobProfileService;
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            string message;

            _logger.LogInformation($"{nameof(Health)} has been called");

            try
            {
                var isHealthy = await _jobProfiles.Ping();

                if (isHealthy)
                {
                    message = "Document store is available";
                    _logger.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

                    var viewModel = CreateHealthViewModel(message);

                    return this.NegotiateContentResult(viewModel, viewModel.HealthItems);
                }

                _logger.LogError($"{nameof(Health)}: Ping to {resourceName} has failed");
            }
            catch (Exception ex)
            {
                message = $"{resourceName} exception: {ex.Message}";
                _logger.LogError($"{nameof(Health)}: {message}");
            }

            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        [HttpGet]
        public IActionResult Ping()
        {
            _logger.LogInformation($"{nameof(Ping)} has been called");

            return Ok();
        }

        private HealthViewModel CreateHealthViewModel(string message)
        {
            return new HealthViewModel
            {
                HealthItems = new List<HealthItemViewModel>
                {
                    new HealthItemViewModel
                    {
                        Service = resourceName,
                        Message = message,
                    },
                },
            };
        }
    }
}
