using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using DFC.App.JobProfile.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfile.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Functions
{
    public static class JobProfileSegmentRefresh
    {
        private const string MessageAction = "ActionType";
        private const string MessageContentType = "CType";
        private const string MessageContentId = "Id";
        private static readonly string ThisClassPath = typeof(JobProfileSegmentRefresh).FullName;

        [FunctionName("JobProfileSegmentRefresh")]
        public static async Task Run(
                                        [ServiceBusTrigger("%job-profiles-refresh-topic%", "%job-profiles-refresh-subscription%", Connection = "service-bus-connection-string")] Message segmentRefreshMessage,
                                        ILogger log,
                                        [Inject] IMessageProcessor processor)
        {
            if (segmentRefreshMessage is null)
            {
                throw new System.ArgumentNullException(nameof(segmentRefreshMessage));
            }

            if (processor is null)
            {
                throw new System.ArgumentNullException(nameof(processor));
            }

            segmentRefreshMessage.UserProperties.TryGetValue(MessageAction, out var messageAction); // Parse to enum values
            segmentRefreshMessage.UserProperties.TryGetValue(MessageContentType, out var messageCtype);
            segmentRefreshMessage.UserProperties.TryGetValue(MessageContentId, out var messageContentId);

            // loggger should allow setting up correlation id and should be picked up from message
            log.LogInformation($"{ThisClassPath}: Received message action {messageAction} for type {messageCtype} with Id: {messageContentId}: Correlation id {segmentRefreshMessage.CorrelationId}");

            var message = Encoding.UTF8.GetString(segmentRefreshMessage.Body);

            //Check whether we need to defer failed messages?
            await processor.ProcessSegmentRefresEventAsync(message).ConfigureAwait(false);
        }
    }
}