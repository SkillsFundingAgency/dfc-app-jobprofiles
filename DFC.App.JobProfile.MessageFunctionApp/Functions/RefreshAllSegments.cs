using System;
using System.Threading.Tasks;
using DFC.App.JobProfile.MessageFunctionApp.Services;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.MessageFunctionApp.Functions
{
    public class RefreshAllSegments
    {
        private readonly ILogService log;
        private readonly IRefreshService refreshService;

        public RefreshAllSegments(ILogService log, IRefreshService refreshService)
        {
            this.log = log;
            this.refreshService = refreshService;
        }

        [FunctionName("RefreshAllSegments")]
        public async Task RunAsync([TimerTrigger("0 0 3 * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{nameof(RefreshCourses)}: Timer trigger function executed at: {DateTime.Now}");
            var run = await refreshService.RefreshAllSegmentsAsync();
            log.LogInformation($"{nameof(RefreshCourses)}: Timer trigger function completed at: {DateTime.Now}");
        }
    }
}
