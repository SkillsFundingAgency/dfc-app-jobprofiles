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

        public virtual async Task<TModel> LoadDataAsync()
        {
            var endpoint = SegmentClientOptions.Endpoint.Replace("{0}", DocumentId.ToString().ToLowerInvariant(), System.StringComparison.OrdinalIgnoreCase);
            var url = $"{SegmentClientOptions.BaseAddress}{endpoint}";

            logger.LogInformation($"{nameof(LoadDataAsync)}: Loading data segment from {url}");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            try
            {
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var result = JsonConvert.DeserializeObject<TModel>(responseString);

                    logger.LogInformation($"{nameof(LoadDataAsync)}: Loaded data segment from {url}");

                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(LoadDataAsync)}: {ex.Message}");
            }

            return default(TModel);
        }

        public virtual async Task<string> LoadMarkupAsync()
        {
            var endpoint = SegmentClientOptions.Endpoint.Replace("{0}", DocumentId.ToString().ToLowerInvariant(), System.StringComparison.OrdinalIgnoreCase);
            var url = $"{SegmentClientOptions.BaseAddress}{endpoint}";

            logger.LogInformation($"{nameof(LoadDataAsync)}: Loading markup segment from {url}");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            try
            {
                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    logger.LogInformation($"{nameof(LoadDataAsync)}: Loaded markup segment from {url}");

                    return responseString;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(LoadMarkupAsync)}: {ex.Message}");
            }

            return null;
        }

        public virtual async Task<HealthCheckItems> HealthCheckAsync()
        {
            var url = $"{SegmentClientOptions.BaseAddress}health";

            logger.LogInformation($"{nameof(LoadDataAsync)}: Checking health for {url}");

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

                    logger.LogInformation($"{nameof(LoadDataAsync)}: Checked health for {url}");

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
    }
}
