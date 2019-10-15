using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.PatchModels;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using DFC.App.JobProfile.MessageFunctionApp.HttpClientPolicies;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Services
{
    public static class OldHttpClientService
    {
        public static async Task<JobProfileModel> GetByIdAsync(HttpClient httpClient, JobProfileClientOptions jobProfileClientOptions, Guid id)
        {
            var endpoint = jobProfileClientOptions.GetEndpoint.Replace("{0}", id.ToString().ToLowerInvariant(), System.StringComparison.OrdinalIgnoreCase);
            var url = $"{jobProfileClientOptions.BaseAddress}{endpoint}";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var result = JsonConvert.DeserializeObject<JobProfileModel>(responseString);

                    return result;
                }
            }

            return default(JobProfileModel);
        }

        public static async Task<HttpStatusCode> PatchAsync(HttpClient httpClient, JobProfileClientOptions jobProfileClientOptions, JobProfileMetaDataPatchModel jobProfileMetaDataPatchModel, Guid documentId)
        {
            var endpoint = jobProfileClientOptions.PatchEndpoint.Replace("{0}", documentId.ToString().ToLowerInvariant(), System.StringComparison.OrdinalIgnoreCase);
            var url = $"{jobProfileClientOptions.BaseAddress}{endpoint}";

            using (var request = new HttpRequestMessage(HttpMethod.Patch, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                request.Content = new ObjectContent(typeof(JobProfileMetaDataPatchModel), jobProfileMetaDataPatchModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json);

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                return response.StatusCode;
            }
        }

        public static async Task<HttpStatusCode> PostAsync(HttpClient httpClient, JobProfileClientOptions jobProfileClientOptions, RefreshJobProfileSegmentModel refreshJobProfileSegmentModel)
        {
            var endpoint = jobProfileClientOptions.PostRefreshEndpoint;
            var url = $"{jobProfileClientOptions.BaseAddress}{endpoint}";

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                request.Content = new ObjectContent(typeof(RefreshJobProfileSegmentModel), refreshJobProfileSegmentModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json);

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                return response.StatusCode;
            }
        }

        public static async Task<HttpStatusCode> PostAsync(HttpClient httpClient, JobProfileClientOptions jobProfileClientOptions, JobProfileModel jobProfileModel)
        {
            var endpoint = jobProfileClientOptions.PostEndpoint;
            var url = $"{jobProfileClientOptions.BaseAddress}{endpoint}";

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                request.Content = new ObjectContent(typeof(RefreshJobProfileSegmentServiceBusModel), jobProfileModel, new JsonMediaTypeFormatter(), MediaTypeNames.Application.Json);

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                return response.StatusCode;
            }
        }

        public static async Task<HttpStatusCode> DeleteAsync(HttpClient httpClient, JobProfileClientOptions jobProfileClientOptions, Guid id)
        {
            var endpoint = jobProfileClientOptions.DeleteEndpoint.Replace("{0}", id.ToString().ToLowerInvariant(), System.StringComparison.OrdinalIgnoreCase);
            var url = $"{jobProfileClientOptions.BaseAddress}{endpoint}";

            using (var request = new HttpRequestMessage(HttpMethod.Delete, url))
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);

                return response.StatusCode;
            }
        }
    }
}
