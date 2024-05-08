using DFC.App.JobProfile.Data.Configuration;
using DFC.App.JobProfile.Data.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.AVService
{
    public class ApprenticeshipVacancyApi : IApprenticeshipVacancyApi
    {
        private readonly ILogger<ApprenticeshipVacancyApi> logger;
        private readonly AVAPIServiceSettings aVAPIServiceSettings;
        private readonly HttpClient httpClient;

        public ApprenticeshipVacancyApi(ILogger<ApprenticeshipVacancyApi> logger, AVAPIServiceSettings aVAPIServiceSettings, HttpClient httpClient)
        {
            this.logger = logger;
            this.aVAPIServiceSettings = aVAPIServiceSettings ?? throw new ArgumentNullException(nameof(aVAPIServiceSettings));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", aVAPIServiceSettings.FAASubscriptionKey);
            this.httpClient.DefaultRequestHeaders.Add("X-Version", "1");
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.Timeout = TimeSpan.FromSeconds(aVAPIServiceSettings.RequestTimeOutSeconds);
        }

        public async Task<string> GetAsync(string requestQueryString, RequestType requestType)
        {
            var queryStringOperator = "?";
            if (requestType == RequestType.VacancyByReference)
            {
                queryStringOperator = "/";
            }

            var fullRequest = $"{aVAPIServiceSettings.FAAEndPoint}{queryStringOperator}{requestQueryString}";

            logger.LogInformation($"Getting API data for request :'{fullRequest}'");

            var response = await httpClient.GetAsync(new Uri(fullRequest)).ConfigureAwait(false);

            if (response == null || response.Content == null)
            {
                logger.LogError($"Response or its content is null for request: '{fullRequest}'");
                throw new HttpRequestException();
            }

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error status {response.StatusCode}, Getting API data for request: '{fullRequest}' \nResponse: {responseContent}");
                response.EnsureSuccessStatusCode();
            }

            return responseContent;
        }
    }
}