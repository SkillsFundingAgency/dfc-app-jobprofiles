﻿using DFC.App.JobProfile.Data.Providers;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.ViewModels;
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
        private readonly ILogger<HealthController> logService;
        private readonly IProvideJobProfiles jobProfileService;
        private readonly AutoMapper.IMapper mapper;
        private readonly string resourceName = typeof(Program).Namespace!;

        public HealthController(ILogger<HealthController> logService, IProvideJobProfiles jobProfileService, AutoMapper.IMapper mapper)
        {
            this.logService = logService;
            this.jobProfileService = jobProfileService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            string message;

            logService.LogInformation($"{nameof(Health)} has been called");

            try
            {
                var isHealthy = await jobProfileService.Ping().ConfigureAwait(false);

                if (isHealthy)
                {
                    message = "Document store is available";
                    logService.LogInformation($"{nameof(Health)} responded with: {resourceName} - {message}");

                    var viewModel = CreateHealthViewModel(message);

                    return this.NegotiateContentResult(viewModel, viewModel.HealthItems);
                }

                logService.LogError($"{nameof(Health)}: Ping to {resourceName} has failed");
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
            logService.LogInformation($"{nameof(Ping)} has been called");

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
