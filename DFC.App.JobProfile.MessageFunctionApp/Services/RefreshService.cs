using DFC.App.JobProfile.Data.Models.CurrentOpportunities;
using DFC.App.JobProfile.MessageFunctionApp.HttpClientPolicies;
using DFC.FindACourseClient;
using DFC.Logger.AppInsights.Constants;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DFC.App.JobProfile.MessageFunctionApp.Services
{
    public class RefreshService : IRefreshService
    {
        private readonly HttpClient httpClient;
        private readonly ILogService logService;
        private readonly JobProfileClientOptions jobProfileClientOptions;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public RefreshService(
                    HttpClient httpClient,
                    ILogService logService,
                    JobProfileClientOptions jobProfileClientOptions,
                    ICorrelationIdProvider correlationIdProvider)
        {
            this.httpClient = httpClient;
            this.logService = logService;
            this.jobProfileClientOptions = jobProfileClientOptions;
            this.correlationIdProvider = correlationIdProvider;
        }

        public async Task<HttpStatusCode> RefreshApprenticeshipsAsync(int retryCount = 0)
        {
            var url = new Uri($"{jobProfileClientOptions.BaseAddress}refreshApprenticeships");
            ConfigureHttpClient();

            var response = await httpClient.PostAsync(url, null).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logService.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for POST Apprenticeships.");

                if (response.StatusCode == HttpStatusCode.PreconditionFailed && retryCount <= 5)
                {
                    return await RefreshApprenticeshipsAsync(retryCount++).ConfigureAwait(false);
                }

                response.EnsureSuccessStatusCode();
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> RefreshAllSegmentsAsync(int first, int skip, int retryCount = 0)
        {
            var url = new Uri($"{jobProfileClientOptions.BaseAddress}refreshAllSegments");
            ConfigureHttpClient();

            var json = JsonConvert.SerializeObject(new JobProfileCurrentOpportunitiesSearchModel { First = first, Skip = skip });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            logService.LogInformation($"{nameof(RefreshAllSegmentsAsync)}: Refresh jobprofile starting from {url} at {skip} to {skip + first}");
            logService.LogInformation($"{nameof(RefreshAllSegmentsAsync)}: Json string is  {content}");

            var response = await httpClient.PostAsync(url, content).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                logService.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for POST Refresh Courses.");

                if (response.StatusCode == HttpStatusCode.PreconditionFailed && retryCount <= 5)
                {
                    return await RefreshAllSegmentsAsync(first, skip, retryCount++).ConfigureAwait(false);
                }

                response.EnsureSuccessStatusCode();
            }

            logService.LogInformation($"{nameof(RefreshAllSegmentsAsync)}: Refresh jobprofile completed from {url} at {skip} to {skip + first}");

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> RefreshCoursesAsync(int retryCount = 0)
        {
            var url = new Uri($"{jobProfileClientOptions.BaseAddress}refreshCourses");
            ConfigureHttpClient();

            var response = await httpClient.PostAsync(url, null).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logService.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for POST Refresh Courses.");

                if (response.StatusCode == HttpStatusCode.PreconditionFailed && retryCount <= 5)
                {
                    return await RefreshCoursesAsync(retryCount++).ConfigureAwait(false);
                }

                response.EnsureSuccessStatusCode();
            }

            return response.StatusCode;
        }



        public async Task<int> CountJobProfiles(int retryCount = 0)
        {
            var url = new Uri($"{jobProfileClientOptions.BaseAddress}countJobProfiles");
            ConfigureHttpClient();

            var response = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<int>(responseString);

                logService.LogInformation($"{nameof(CountJobProfiles)}: Get jobprofile count {result} from {url}");

                return result;
            }

            logService.LogError($"{nameof(CountJobProfiles)}: Error Get jobprofile count from {url}, status: {response.StatusCode}");

            return 0;
        }

        private void ConfigureHttpClient()
        {
            logService.LogInformation($"{nameof(ConfigureHttpClient)} has been called");

            if (!httpClient.DefaultRequestHeaders.Contains(HeaderName.CorrelationId))
            {
                logService.LogInformation($"{nameof(ConfigureHttpClient)} does not contain {nameof(HeaderName.CorrelationId)}");

                httpClient.DefaultRequestHeaders.Add(HeaderName.CorrelationId, correlationIdProvider.CorrelationId);
                httpClient.Timeout = TimeSpan.FromMinutes(30);
            }
        }
    }
}
