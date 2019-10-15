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
    public static class SitefinityMessageHandler
    {
        private const string MessageAction = "ActionType";
        private const string MessageContentType = "CType";
        private const string MessageContentId = "Id";
        private static readonly string ThisClassPath = typeof(SitefinityMessageHandler).FullName;


        [FunctionName("SitefinityMessageHandler")]
        public static async Task Run(
                                        [ServiceBusTrigger("%cms-messages-topic%", "%cms-messages-subscription%", Connection = "service-bus-connection-string")] Message sitefinityMessage,
                                        ILogger log,
                                        IMessageProcessor processor,
                                        [Inject] JobProfileClientOptions jobProfileClientOptions,
                                        [Inject] HttpClient httpClient)
        {
            if (sitefinityMessage != null)
            {
                sitefinityMessage.UserProperties.TryGetValue(MessageAction, out var messageAction); // Parse to enum values
                sitefinityMessage.UserProperties.TryGetValue(MessageContentType, out var messageCtype);
                sitefinityMessage.UserProperties.TryGetValue(MessageContentId, out var messageContentId);

                // loggger should allow setting up correlation id and should be picked up from message
                log.LogInformation($"{ThisClassPath}: Received message action {messageAction} for type {messageCtype} with Id: {messageContentId}: Correlation id {sitefinityMessage.CorrelationId}");

                var message = Encoding.UTF8.GetString(sitefinityMessage.Body);
                var result = await processor.ProcessAsync(message, messageAction.ToString(), messageCtype.ToString(), messageContentId.ToString(), sitefinityMessage.SystemProperties.SequenceNumber).ConfigureAwait(false);

                if (result == HttpStatusCode.OK)
                {
                    log.LogInformation($"{ThisClassPath}: JobProfile Id: {messageContentId}: Updated profile");
                }
                else if (result == HttpStatusCode.Created)
                {
                    log.LogInformation($"{ThisClassPath}: JobProfile Id: {messageContentId}: Created profile");
                }
                else
                {
                    log.LogWarning($"{ThisClassPath}: JobProfile Id: {messageContentId}: Profile not Posted: Status: {result}");
                }
            }
        }
    }
}
