using DFC.App.JobProfile.MessageFunctionApp.Services;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Functions
{
    public class RefreshApprenticeships
    {
        private readonly ILogService log;
        private readonly IRefreshService refreshService;

        public RefreshApprenticeships(ILogService log, IRefreshService refreshService)
        {
            this.log = log;
            this.refreshService = refreshService;
        }

        [FunctionName("RefreshApprenticeships")]
        public async Task RunAsync([TimerTrigger("0 0 4 * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function executed at: {DateTime.Now}");
            var run = await refreshService.RefreshApprenticeshipsAsync();
            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function completed at: {DateTime.Now}");
        }
    }
}
