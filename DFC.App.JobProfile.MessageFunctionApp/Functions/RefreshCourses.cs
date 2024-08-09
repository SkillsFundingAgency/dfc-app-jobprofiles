using DFC.App.JobProfile.MessageFunctionApp.Services;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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
        public async Task RunAsync([TimerTrigger("%RefreshCoursesTriggerTimer%", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function executed at: {DateTime.Now}");
            int first = 100;
            int skip = 0;
            int count = await refreshService.CountJobProfiles();
            log.LogInformation($"{nameof(RefreshApprenticeships)}: Total Jobprofiles count is: {count}");

            while (count > 0 && skip < count)
            {
                log.LogInformation($"{nameof(RefreshApprenticeships)}: Start processing from {skip} to {skip + first}");
                await refreshService.RefreshCoursesAsync(first, skip);
                log.LogInformation($"{nameof(RefreshApprenticeships)}: Finish processing from {skip} to {skip + first}");
                skip += first;
            }

            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function completed at: {DateTime.Now}");
        }
    }
}
