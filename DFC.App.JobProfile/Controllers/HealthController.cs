using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    public class HealthController : Controller
    {
        private readonly ILogService logService;
        private readonly IJobProfileService jobProfileService;
        private readonly AutoMapper.IMapper mapper;

        public HealthController(ILogService logService, IJobProfileService jobProfileService, AutoMapper.IMapper mapper)
        {
            this.logService = logService;
            this.jobProfileService = jobProfileService;
            this.mapper = mapper;
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
                var isHealthy = await jobProfileService.PingAsync().ConfigureAwait(false);

                if (isHealthy)
                {
                    message = "Document store is available";
                    logService.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

                    var viewModel = CreateHealthViewModel(resourceName, message);

                    var segmentData = await jobProfileService.SegmentsHealthCheckAsync().ConfigureAwait(false);
                    var segmentHealthItemViewModels = mapper.Map<List<HealthItemViewModel>>(segmentData);

                    viewModel.HealthItems.AddRange(segmentHealthItemViewModels);

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
        [Route("health/ping")]
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
