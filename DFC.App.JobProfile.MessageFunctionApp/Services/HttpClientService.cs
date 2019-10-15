using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.PatchModels;
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
    public class HttpClientService : IHttpClientService<JobProfileMetaDataPatchModel>
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

        public async Task<JobProfileMetaDataPatchModel> GetByIdAsync(Guid id)
        {
            var url = new Uri($"{jobProfileClientOptions.BaseAddress}/profile/{id}");
            var response = await httpClient.GetAsync(url).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<JobProfileMetaDataPatchModel>(responseString);

                return result;
            }

            return default;
        }

        public async Task<HttpStatusCode> PatchAsync<T>(T patchModel, string patchTypeEndpoint)
            where T : BasePatchModel
        {
            if (patchModel is null)
            {
                throw new ArgumentNullException(nameof(patchModel));
            }

            if (string.IsNullOrWhiteSpace(patchTypeEndpoint))
            {
                throw new ArgumentException("message", nameof(patchTypeEndpoint));
            }

            var url = new Uri($"{jobProfileClientOptions.BaseAddress}/profile/{patchModel?.JobProfileId}/{patchTypeEndpoint}");
            using (var content = new ObjectContent<T>(patchModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json))
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
            if (httpClient == null)
            {
                return default;
            }

            var url = new Uri($"{jobProfileClientOptions.BaseAddress}/profile/{id}");
            var response = await httpClient.DeleteAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for DELETE, Id: {id}");

                response.EnsureSuccessStatusCode();
            }

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> PostAsync(JobProfileMetaDataPatchModel jobProfile)
        {
            if (jobProfile is null)
            {
                throw new ArgumentNullException(nameof(jobProfile));
            }

            var url = new Uri($"{jobProfileClientOptions.BaseAddress}/profile");
            using (var content = new ObjectContent<JobProfileMetaDataPatchModel>(jobProfile, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json))
            {
                var response = await httpClient.PostAsync(url, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    logger.LogError($"Failure status code '{response.StatusCode}' received with content '{responseContent}', for POST, Id: {jobProfile.JobProfileId}, name: {jobProfile.CanonicalName}");

                    response.EnsureSuccessStatusCode();
                }

                return response.StatusCode;
            }
        }
    }
}