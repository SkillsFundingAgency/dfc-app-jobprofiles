using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService.SegmentServices
{
    public abstract class BaseSegmentService<TModel, TService> : IBaseSegmentService<TModel>
        where TModel : ISegmentDataModel
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<TService> logger;

        public BaseSegmentService(HttpClient httpClient, ILogger<TService> logger, SegmentClientOptions segmentClientOptions)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.SegmentClientOptions = segmentClientOptions;
        }

        public Guid DocumentId { get; set; }

        public SegmentClientOptions SegmentClientOptions { get; set; }

        public virtual async Task<HealthCheckItems> HealthCheckAsync()
        {
            var url = $"{SegmentClientOptions.BaseAddress}health";

            logger.LogInformation($"{nameof(HealthCheckAsync)}: Checking health for {url}");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var result = JsonConvert.DeserializeObject<HealthCheckItems>(responseString);

                    result.Source = SegmentClientOptions.BaseAddress;

                    logger.LogInformation($"{nameof(HealthCheckAsync)}: Checked health for {url}");

                    return result;
                }
                else
                {
                    logger.LogError($"{nameof(HealthCheckAsync)}: Error loading health data from {url}: {response.StatusCode}");

                    var result = new HealthCheckItems
                    {
                        Source = SegmentClientOptions.BaseAddress,
                        HealthItems = new List<HealthCheckItem>
                        {
                            new HealthCheckItem
                            {
                                Service = SegmentClientOptions.BaseAddress.ToString(),
                                Message = $"No health response from {SegmentClientOptions.BaseAddress.ToString()} app",
                            },
                        },
                    };

                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(HealthCheckAsync)}: {ex.Message}");

                var result = new HealthCheckItems
                {
                    Source = SegmentClientOptions.BaseAddress,
                    HealthItems = new List<HealthCheckItem>
                    {
                        new HealthCheckItem
                        {
                            Service = SegmentClientOptions.BaseAddress.ToString(),
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
            var endpoint = string.Format(SegmentClientOptions.Endpoint, jobProfileId.ToString());
            var url = $"{SegmentClientOptions.BaseAddress}{endpoint}";

            logger.LogInformation($"{nameof(GetJsonAsync)}: Loading data segment from {url}");

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(acceptHeader));

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"Failed to get {acceptHeader} data for {jobProfileId} from {url}, received error : {responseString}");
                }
                else if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logger.LogInformation($"Status - {response.StatusCode} received for {jobProfileId} from {url}, Returning empty content.");
                    return null;
                }

                return responseString;
            }
        }
    }
}