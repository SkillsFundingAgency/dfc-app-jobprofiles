using DFC.App.JobProfile.CurrentOpportunities.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DFC.Logger.AppInsights.Contracts;

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
        public async Task RunAsync(
            [TimerTrigger("%RefreshApprenticeshipsCron%")] TimerInfo myTimer)
        {
            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function starting at: {DateTime.Now}, using TimerInfo: {myTimer.Schedule.ToString()}");

            int abortAfterErrorCount = 10;
            int errorCount = 0;
            int totalErrorCount = 0;
            int totalSuccessCount = 0;
            int aVRequestsPerMinute = 240;
            int aVRequestsPerMinuteSettingOveride = 0;

            _ = int.TryParse(Environment.GetEnvironmentVariable(nameof(abortAfterErrorCount)), out abortAfterErrorCount);
            _ = int.TryParse(Environment.GetEnvironmentVariable(nameof(aVRequestsPerMinuteSettingOveride)), out aVRequestsPerMinuteSettingOveride);

            //override with a setting variable if required
            aVRequestsPerMinute = aVRequestsPerMinuteSettingOveride > 0 ? aVRequestsPerMinuteSettingOveride : aVRequestsPerMinute;

            var sleepTimeMilliSecsBetweenRequests = 60000 / (aVRequestsPerMinute / 3);   //on average we make 3 calls per profile to get 2 vacancies, so divide by 3

            HttpStatusCode statusCode = HttpStatusCode.OK;

            var simpleJobProfileModels = await refreshService.GetListAsync().ConfigureAwait(false);

            if (simpleJobProfileModels != null)
            {
                log.LogInformation($"{nameof(RefreshApprenticeships)}: Retrieved {simpleJobProfileModels.Count} Job Profiles");

                foreach (var simpleJobProfileModel in simpleJobProfileModels)
                {
                    log.LogInformation($"{nameof(RefreshApprenticeships)}: Refreshing Job Profile Apprenticeships: {simpleJobProfileModel.DocumentId} / {simpleJobProfileModel.CanonicalName}");

                    await Task.Delay(sleepTimeMilliSecsBetweenRequests).ConfigureAwait(false);

                    statusCode = await refreshService.RefreshApprenticeshipsAsync(simpleJobProfileModel.DocumentId).ConfigureAwait(false);

                    switch (statusCode)
                    {
                        case HttpStatusCode.OK:
                            errorCount = 0;
                            totalSuccessCount++;
                            log.LogInformation($"{nameof(RefreshApprenticeships)}: Refreshed Job Profile Apprenticeships: {simpleJobProfileModel.DocumentId} / {simpleJobProfileModel.CanonicalName}");
                            break;

                        default:
                            errorCount++;
                            totalErrorCount++;
                            log.LogError($"{nameof(RefreshApprenticeships)}: Error refreshing Job Profile Apprenticeships: {simpleJobProfileModel.DocumentId} / {simpleJobProfileModel.CanonicalName} - Status code = {statusCode}");
                            break;
                    }

                    if (errorCount >= abortAfterErrorCount)
                    {
                        log.LogWarning($"{nameof(RefreshApprenticeships)}: Timer trigger aborting after {abortAfterErrorCount} consecutive errors");
                        break;
                    }
                }
            }

            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function, Apprenticeships refreshed: {totalSuccessCount}");
            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function, Apprenticeships refresh errors: {totalErrorCount}");
            log.LogInformation($"{nameof(RefreshApprenticeships)}: Timer trigger function completed at: {DateTime.Now}");

            // if we aborted due to the number of errors exceeding the abortAfterErrorCount
            if (errorCount >= abortAfterErrorCount)
            {
                throw new HttpResponseException(new HttpResponseMessage() { StatusCode = statusCode, ReasonPhrase = $"Timer trigger aborting after {abortAfterErrorCount} consecutive errors" });
            }
        }
    }
}
