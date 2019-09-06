using DFC.App.JobProfile.Data.HttpClientPolicies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
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

        public SegmentClientOptions SegmentClientOptions { get; set; }

        public virtual async Task<TModel> LoadAsync(string article)
        {
            var endpoint = SegmentClientOptions.Endpoint.Replace("{0}", article, System.StringComparison.OrdinalIgnoreCase);
            var url = $"{SegmentClientOptions.BaseAddress}{endpoint}";

            logger.LogInformation($"{nameof(LoadAsync)}: Loading segment from {url}");

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var result = JsonConvert.DeserializeObject<TModel>(responseString);

                logger.LogInformation($"{nameof(LoadAsync)}: Loaded segment from {url}");

                return result;
            }

            return default(TModel);
        }
    }
}
