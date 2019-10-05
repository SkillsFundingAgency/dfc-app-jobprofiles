using DFC.App.JobProfile.Data.Models;
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
            var serviceBusModel = JsonConvert.DeserializeObject<JobProfileMetaDataPatchServiceBusModel>(serviceBusMessage);

            log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Patching job profile");

            var jobProfileModel = await HttpClientService.GetByIdAsync(httpClient, jobProfileClientOptions, serviceBusModel.JobProfileId).ConfigureAwait(false);

            if (jobProfileModel != null)
            {
                if (jobProfileModel.LastReviewed < serviceBusModel.LastReviewed)
                {
                    var jobProfileMetaDataPatchModel = new JobProfileMetaDataPatchModel
                    {
                        CanonicalName = serviceBusModel.CanonicalName,
                        BreadcrumbTitle = serviceBusModel.BreadcrumbTitle,
                        IncludeInSitemap = serviceBusModel.IncludeInSitemap,
                        AlternativeNames = serviceBusModel.AlternativeNames,
                        MetaTags = new MetaTagsModel
                        {
                            Title = serviceBusModel.Title,
                            Description = serviceBusModel.Description,
                            Keywords = serviceBusModel.Keywords,
                        },
                    };

                    var result = await HttpClientService.PatchAsync(httpClient, jobProfileClientOptions, jobProfileMetaDataPatchModel, serviceBusModel.JobProfileId).ConfigureAwait(false);

                    if (result == HttpStatusCode.OK)
                    {
                        log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Patched job profile");
                    }
                    else
                    {
                        log.LogWarning($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Job profile not patched: Status: {result}");
                    }
                }
                else
                {
                    log.LogWarning($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Service bus message is stale: {serviceBusModel.LastReviewed}, stored: {jobProfileModel.LastReviewed}");
                }
            }
            else
            {
                jobProfileModel = new JobProfileModel
                {
                    DocumentId = serviceBusModel.JobProfileId,
                    SocLevelTwo = serviceBusModel.SocLevelTwo,
                    CanonicalName = serviceBusModel.CanonicalName,
                    LastReviewed = serviceBusModel.LastReviewed,
                    BreadcrumbTitle = serviceBusModel.BreadcrumbTitle,
                    IncludeInSitemap = serviceBusModel.IncludeInSitemap,
                    AlternativeNames = serviceBusModel.AlternativeNames,
                    MetaTags = new MetaTagsModel
                    {
                        Title = serviceBusModel.Title,
                        Description = serviceBusModel.Description,
                        Keywords = serviceBusModel.Keywords,
                    },
                };

                var result = await HttpClientService.PostAsync(httpClient, jobProfileClientOptions, jobProfileModel).ConfigureAwait(false);

                if (result == HttpStatusCode.OK)
                {
                    log.LogInformation($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Posted job profile");
                }
                else
                {
                    log.LogWarning($"{ThisClassPath}: JobProfile Id: {serviceBusModel.JobProfileId}: Job profile not posted: Status: {result}");
                }
            }
        }
    }
}
