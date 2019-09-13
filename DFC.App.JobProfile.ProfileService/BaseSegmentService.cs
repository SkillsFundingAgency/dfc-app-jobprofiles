using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService
{
    public abstract class BaseSegmentService<TModel, TService> : IBaseSegmentService<TModel>
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<TService> logger;

        public BaseSegmentService(HttpClient httpClient, ILogger<TService> logger, SegmentClientOptions segmentClientOptions)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.SegmentClientOptions = segmentClientOptions;
        }

        public string CanonicalName { get; set; }

        public SegmentClientOptions SegmentClientOptions { get; set; }

        public virtual async Task<TModel> LoadDataAsync()
        {
            var endpoint = SegmentClientOptions.Endpoint.Replace("{0}", CanonicalName, System.StringComparison.OrdinalIgnoreCase);
            var url = $"{SegmentClientOptions.BaseAddress}{endpoint}";

            logger.LogInformation($"{nameof(LoadDataAsync)}: Loading data segment from {url}");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var result = JsonConvert.DeserializeObject<TModel>(responseString);

                logger.LogInformation($"{nameof(LoadDataAsync)}: Loaded data segment from {url}");

                return result;
            }

            return default(TModel);
        }

        public virtual async Task<string> LoadMarkupAsync()
        {
            var endpoint = SegmentClientOptions.Endpoint.Replace("{0}", CanonicalName, System.StringComparison.OrdinalIgnoreCase);
            var url = $"{SegmentClientOptions.BaseAddress}{endpoint}";

            logger.LogInformation($"{nameof(LoadDataAsync)}: Loading markup segment from {url}");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                logger.LogInformation($"{nameof(LoadDataAsync)}: Loaded markup segment from {url}");

                return responseString;
            }

            return null;
        }

        public virtual async Task<IList<HealthCheckItem>> HealthCheckAsync()
        {
            var url = $"{SegmentClientOptions.BaseAddress}health";

            logger.LogInformation($"{nameof(LoadDataAsync)}: Checking health for {url}");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var result = JsonConvert.DeserializeObject<HealthCheckItems>(responseString);

                logger.LogInformation($"{nameof(LoadDataAsync)}: Checked health for {url}");

                return result.HealthItems;
            }

            return default(List<HealthCheckItem>);
        }
    }
}
