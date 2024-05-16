using System;
using System.Threading.Tasks;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.MessageFunctionApp.Services;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DFC.App.JobProfile.MessageFunctionApp.Functions
{
    public class RefreshCourses
    {
        private readonly ILogService log;
        private readonly IRefreshService refreshService;

        public RefreshCourses(ILogService log, IRefreshService refreshService)
        {
            this.log = log;
            this.refreshService = refreshService;
        }

        [FunctionName("RefreshCourses")]
        public async Task RunAsync([TimerTrigger("0 0 5 * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{nameof(RefreshCourses)}: Timer trigger function executed at: {DateTime.Now}");
            var run = await refreshService.RefreshCoursesAsync();
            log.LogInformation($"{nameof(RefreshCourses)}: Timer trigger function completed at: {DateTime.Now}");
        }
    }
}
