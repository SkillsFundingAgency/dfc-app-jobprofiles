using Azure.Messaging.ServiceBus;
using DFC.App.JobProfile.MessageFunctionApp.Services;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Functions
{
    public class SitefinityMessageHandler
    {
        private const string MessageAction = "ActionType";
        private const string MessageContentType = "CType";
        private const string MessageContentId = "Id";
        private readonly string thisClassPath = typeof(SitefinityMessageHandler).FullName;
        private readonly ILogService logService;
        private readonly IMessageProcessor processor;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public SitefinityMessageHandler(
            ILogService logService,
            IMessageProcessor processor,
            ICorrelationIdProvider correlationIdProvider)
        {
            this.logService = logService;
            this.processor = processor;
            this.correlationIdProvider = correlationIdProvider;
        }

        [FunctionName("SitefinityMessageHandler")]
        public async Task Run([ServiceBusTrigger("%cms-messages-topic%", "%cms-messages-subscription%", Connection = "service-bus-connection-string")] Message sitefinityMessage)
        {
            if (sitefinityMessage is null)
            {
                throw new System.ArgumentNullException(nameof(sitefinityMessage));
            }

            if (processor is null)
            {
                throw new System.ArgumentNullException(nameof(processor));
            }

            if (correlationIdProvider != null)
            {
                correlationIdProvider.CorrelationId = sitefinityMessage.CorrelationId;
            }

            sitefinityMessage.UserProperties.TryGetValue(MessageAction, out var messageAction); // Parse to enum values
            sitefinityMessage.UserProperties.TryGetValue(MessageContentType, out var messageCtype);
            sitefinityMessage.UserProperties.TryGetValue(MessageContentId, out var messageContentId);

            // loggger should allow setting up correlation id and should be picked up from message
            logService.LogInformation($"{thisClassPath}: Received message sequence {sitefinityMessage.SystemProperties.SequenceNumber}, action {messageAction} for type {messageCtype} with Id: {messageContentId}: Correlation id {sitefinityMessage.CorrelationId}");

            var message = Encoding.UTF8.GetString(sitefinityMessage.Body);
            var result = await processor.ProcessSitefinityMessageAsync(message, messageAction.ToString(), messageCtype.ToString(), messageContentId.ToString(), sitefinityMessage.SystemProperties.SequenceNumber).ConfigureAwait(false);

            if (result == HttpStatusCode.OK)
            {
                logService.LogInformation($"{thisClassPath}: JobProfile Id: {messageContentId}: Updated profile");
            }
            else if (result == HttpStatusCode.Created)
            {
                logService.LogInformation($"{thisClassPath}: JobProfile Id: {messageContentId}: Created profile");
            }
            else
            {
                logService.LogWarning($"{thisClassPath}: JobProfile Id: {messageContentId}: Profile not Posted: Status: {result}");
            }
        }
    }
}