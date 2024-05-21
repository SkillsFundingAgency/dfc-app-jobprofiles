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
            log.LogInformation($"{nameof(RefreshAllSegments)}: Timer trigger function executed at: {DateTime.Now}");
            int first = 100;
            int skip = 0;
            int count = await refreshService.CountJobProfiles();
            log.LogInformation($"{nameof(RefreshAllSegments)}: Total Jobprofiles count is: {count}");

            while (count > 0 && skip < count)
            {
                log.LogInformation($"{nameof(RefreshAllSegments)}: Start processing from {skip} to {skip + first}");
                await refreshService.RefreshAllSegmentsAsync(first, skip);
                log.LogInformation($"{nameof(RefreshAllSegments)}: Finish processing from {skip} to {skip + first}");
                skip += first;
            }

            log.LogInformation($"{nameof(RefreshAllSegments)}: Timer trigger function completed at: {DateTime.Now}");
        }
    }
}
