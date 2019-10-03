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
    public static class PostJobProfile
    {
        private static readonly string ThisClassPath = typeof(PostJobProfile).FullName;

        [FunctionName("PostJobProfile")]
        public static async Task Run(
                                        [ServiceBusTrigger("%post-profile-topic-name%", "%post-profile-subscription-name%", Connection = "service-bus-connection-string")] string serviceBusMessage,
                                        ILogger log,
                                        [Inject] JobProfileClientOptions jobProfileClientOptions,
                                        [Inject] HttpClient httpClient)
        {
            var serviceBusModel = JsonConvert.DeserializeObject<RefreshJobProfileSegmentServiceBusModel>(serviceBusMessage);

            log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Posting profile");

            var result = await HttpClientService.PostAsync(httpClient, jobProfileClientOptions, serviceBusModel).ConfigureAwait(false);

            if (result == HttpStatusCode.OK)
            {
                log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Updated profile");
            }
            else if (result == HttpStatusCode.Created)
            {
                log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Created profile");
            }
            else
            {
                log.LogWarning($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Profile not Posted: Status: {result}");
            }
        }
    }
}
