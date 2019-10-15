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
    public static class DeleteJobProfile
    {
        private static readonly string ThisClassPath = typeof(DeleteJobProfile).FullName;

        [FunctionName("DeleteJobProfile")]
        public static async Task Run(
                                        [ServiceBusTrigger("%delete-topic-name%", "%delete-subscription-name%", Connection = "service-bus-connection-string")] string serviceBusMessage,
                                        ILogger log,
                                        [Inject] JobProfileClientOptions jobProfileClientOptions,
                                        [Inject] HttpClient httpClient)
        {
            var serviceBusModel = JsonConvert.DeserializeObject<JobProfileDeleteServiceBusModel>(serviceBusMessage);

            log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Deleting job profile");

            var jobProfileDataModel = await OldHttpClientService.GetByIdAsync(httpClient, jobProfileClientOptions, serviceBusModel.JobProfileId).ConfigureAwait(false);

            if (jobProfileDataModel == null || jobProfileDataModel.LastReviewed < serviceBusModel.LastReviewed)
            {
                var result = await OldHttpClientService.DeleteAsync(httpClient, jobProfileClientOptions, serviceBusModel.JobProfileId).ConfigureAwait(false);

                if (result == HttpStatusCode.OK)
                {
                    log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Deleted job profile");
                }
                else
                {
                    log.LogWarning($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Job profile not deleted: Status: {result}");
                }
            }
        }
    }
}
