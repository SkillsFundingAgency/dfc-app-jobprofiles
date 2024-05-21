using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    public class HealthController : Controller
    {
        private readonly ILogService logService;
        private readonly HealthCheckService healthCheckService;

        public HealthController(ILogService logService, HealthCheckService healthCheckService)
        {
            this.logService = logService;
            this.healthCheckService = healthCheckService;
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            string resourceName = typeof(Program).Namespace;
            string message;

            logService.LogInformation($"{nameof(Health)} has been called");

            try
            {
                var report = await healthCheckService.CheckHealthAsync();
                var status = report.Status;

                if (status == HealthStatus.Healthy)
                {
                    message = "Apprenticeship Service, Azure Redis and GraphQl are available";
                    logService.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

                    var viewModel = CreateHealthViewModel(resourceName, message);

                    return this.NegotiateContentResult(viewModel, viewModel.HealthItems);
                }

                message = $"Ping to {resourceName} has failed";
                logService.LogError($"{nameof(Health)}: {message}");
            }
            catch (Exception ex)
            {
                message = $"{resourceName} exception: {ex.Message}";
                logService.LogError($"{nameof(Health)}: {message}");
            }

            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        [HttpGet]
        public IActionResult Ping()
        {
            logService.LogVerbose($"{nameof(Ping)} has been called");

            return Ok();
        }

        private static HealthViewModel CreateHealthViewModel(string resourceName, string message)
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
