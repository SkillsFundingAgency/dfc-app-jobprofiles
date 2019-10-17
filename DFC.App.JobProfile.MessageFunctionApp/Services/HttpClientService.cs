using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.MessageFunctionApp.HttpClientPolicies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Services
{
    public class HttpClientService<T> : IHttpClientService<T>
                where T : class, new()
    {
        private readonly HttpClient httpClient;
        private readonly JobProfileClientOptions jobProfileClientOptions;
        private readonly ILogger logger;

        public HttpClientService(JobProfileClientOptions jobProfileClientOptions, HttpClient httpClient, ILogger logger)
        {
            this.jobProfileClientOptions = jobProfileClientOptions;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var url = new Uri($"{jobProfileClientOptions.BaseAddress}profile/{id}");
            var response = await httpClient.GetAsync(url).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<T>(responseString);

                return result;
            }

            return default;
        }

        public async Task<HttpStatusCode> PatchAsync<TInput>(TInput patchModel, string patchTypeEndpoint)
            where TInput : BaseJobProfile
        {
            if (patchModel is null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            if (string.IsNullOrWhiteSpace(patchTypeEndpoint))
            {
                throw new ArgumentException("message", nameof(patchTypeEndpoint));
            }

            var url = new Uri($"{jobProfileClientOptions.BaseAddress}profile/{patchModel?.JobProfileId}/{patchTypeEndpoint}");
            using (var content = new ObjectContent<TInput>(patchModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json))
            {
                var response = await httpClient.PatchAsync(url, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for patch type {typeof(T)}, Id: {patchModel.JobProfileId}");

                    response.EnsureSuccessStatusCode();
                }

                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> DeleteAsync(Guid id)
        {
            var url = new Uri($"{jobProfileClientOptions.BaseAddress}profile/{id}");
            var response = await httpClient.DeleteAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for DELETE, Id: {id}");

                response.EnsureSuccessStatusCode();
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> PostAsync<TInput>(TInput postModel, string postEndpoint)
            where TInput : BaseJobProfile
        {
            if (postModel is null)
            {
                throw new ArgumentNullException(nameof(postModel));
            }

            var url = new Uri($"{jobProfileClientOptions.BaseAddress}{postEndpoint}");
            using (var content = new ObjectContent<TInput>(postModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json))
            {
                var response = await httpClient.PostAsync(url, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for POST type {typeof(T)}, Id: {postModel.JobProfileId}.");

                    response.EnsureSuccessStatusCode();
                }

                return response.StatusCode;
            }
        }
    }
}