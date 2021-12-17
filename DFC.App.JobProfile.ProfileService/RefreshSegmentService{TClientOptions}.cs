using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using DFC.Logger.AppInsights.Constants;
using DFC.Logger.AppInsights.Contracts;
using Newtonsoft.Json;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService.SegmentServices
{
    public class RefreshSegmentService<TClientOptions> : Data.Contracts.SegmentServices.ISegmentRefreshService<TClientOptions>
        where TClientOptions : SegmentClientOptions
    {
        private readonly HttpClient httpClient;
        private readonly ILogService logService;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public RefreshSegmentService(HttpClient httpClient, ILogService logService, TClientOptions segmentClientOptions, ICorrelationIdProvider correlationIdProvider)
        {
            this.httpClient = httpClient;
            this.logService = logService;
            this.SegmentClientOptions = segmentClientOptions;
            this.correlationIdProvider = correlationIdProvider;
        }

        public TClientOptions SegmentClientOptions { get; set; }

        public virtual async Task<HealthCheckItems> HealthCheckAsync()
        {
            var url = $"{SegmentClientOptions.BaseAddress}health";

            logService.LogInformation($"{nameof(HealthCheckAsync)}: Checking health for {url}");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                ConfigureHttpClient();

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var result = JsonConvert.DeserializeObject<HealthCheckItems>(responseString);

                    result.Source = SegmentClientOptions.BaseAddress;

                    logService.LogInformation($"{nameof(HealthCheckAsync)}: Checked health for {url}");

                    return result;
                }
                else
                {
                    logService.LogError($"{nameof(HealthCheckAsync)}: Error loading health data from {url}: {response.StatusCode}");

                    var aa = SegmentClientOptions.BaseAddress;

                    var result = new HealthCheckItems
                    {
                        Source = SegmentClientOptions.BaseAddress,
                        HealthItems = new List<HealthCheckItem>
                        {
                            new HealthCheckItem
                            {
                                Service = SegmentClientOptions.Name,
                                Message = $"No health response from {SegmentClientOptions.BaseAddress.ToString()} app",
                            },
                        },
                    };

                    return result;
                }
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(HealthCheckAsync)}: {ex.Message}");

                var result = new HealthCheckItems
                {
                    Source = SegmentClientOptions.BaseAddress,
                    HealthItems = new List<HealthCheckItem>
                    {
                        new HealthCheckItem
                        {
                            Service = SegmentClientOptions.Name,
                            Message = $"{ex.GetType().Name}: {ex.Message}",
                        },
                    },
                };

                return result;
            }
        }

        public virtual async Task<string> GetJsonAsync(Guid jobProfileId) => await GetAsync(jobProfileId, MediaTypeNames.Application.Json).ConfigureAwait(false);

        public virtual async Task<string> GetMarkupAsync(Guid jobProfileId) => await GetAsync(jobProfileId, MediaTypeNames.Text.Html).ConfigureAwait(false);

        private async Task<string> GetAsync(Guid jobProfileId, string acceptHeader)
        {
            var endpoint = string.Format(CultureInfo.InvariantCulture, SegmentClientOptions.Endpoint, jobProfileId.ToString());
            var url = $"{SegmentClientOptions.BaseAddress}{endpoint}";

            logService.LogInformation($"{nameof(GetJsonAsync)}: Loading data segment from {url}");

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(acceptHeader));
            ConfigureHttpClient();

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                logService.LogError($"Failed to get {acceptHeader} data for {jobProfileId} from {url}, received error : '{responseString}', Returning empty content.");
                responseString = null;
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                logService.LogInformation($"Status - {response.StatusCode} with response '{responseString}' received for {jobProfileId} from {url}, Returning empty content.");
                responseString = null;
            }

            return responseString;
        }

        private void ConfigureHttpClient()
        {
            if (!httpClient.DefaultRequestHeaders.Contains(HeaderName.CorrelationId))
            {
                httpClient.DefaultRequestHeaders.Add(HeaderName.CorrelationId, correlationIdProvider.CorrelationId);
            }
        }
    }
}