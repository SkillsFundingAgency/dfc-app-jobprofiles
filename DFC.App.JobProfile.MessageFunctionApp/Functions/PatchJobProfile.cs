using DFC.App.JobProfile.Data.Models.PatchModels;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using DFC.App.JobProfile.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfile.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Functions
{
    public static class PatchJobProfile
    {
        private static readonly string ThisClassPath = typeof(PatchJobProfile).FullName;

        [FunctionName("PatchJobProfile")]
        public static async Task Run(
                                        [ServiceBusTrigger("%patch-profile-topic-name%", "%patch-profile-subscription-name%", Connection = "service-bus-connection-string")] string serviceBusMessage,
                                        ILogger log,
                                        [Inject] JobProfileClientOptions jobProfileClientOptions,
                                        [Inject] HttpClient httpClient)
        {
            var serviceBusModel = JsonConvert.DeserializeObject<JobProfilePatchServiceBusModel>(serviceBusMessage);

            log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Patching job profile");

            var jobProfileModel = await HttpClientService.GetByIdAsync(httpClient, jobProfileClientOptions, serviceBusModel.JobProfileId).ConfigureAwait(false);

            if (jobProfileModel == null || jobProfileModel.Data == null)
            {
                var jobProfilePatchModel = new JobProfilePatchModel
                {
                    SocLevelTwo = serviceBusModel.SocLevelTwo,
                    CanonicalName = serviceBusModel.CanonicalName,
                };

                var result = await HttpClientService.PatchAsync(httpClient, jobProfileClientOptions, jobProfilePatchModel, serviceBusModel.JobProfileId).ConfigureAwait(false);

                if (result == HttpStatusCode.OK)
                {
                    log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Patched job profile");
                }
                else
                {
                    log.LogWarning($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Job profile not patched: Status: {result}");
                }
            }
        }
    }
}
