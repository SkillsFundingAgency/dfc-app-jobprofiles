using DFC.App.JobProfile.MessageFunctionApp.HttpClientPolicies;
using DFC.Logger.AppInsights.Constants;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

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

        private void ConfigureHttpClient()
        {
            logService.LogInformation($"{nameof(ConfigureHttpClient)} has been called");

            if (!httpClient.DefaultRequestHeaders.Contains(HeaderName.CorrelationId))
            {
                logService.LogInformation($"{nameof(ConfigureHttpClient)} does not contain {nameof(HeaderName.CorrelationId)}");

                httpClient.DefaultRequestHeaders.Add(HeaderName.CorrelationId, correlationIdProvider.CorrelationId);
                httpClient.Timeout = TimeSpan.FromMinutes(5);
            }
        }
    }
}
