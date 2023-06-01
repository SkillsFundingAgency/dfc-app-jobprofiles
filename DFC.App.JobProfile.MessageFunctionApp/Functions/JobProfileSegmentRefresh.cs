using Azure.Messaging.ServiceBus;
using DFC.App.JobProfile.MessageFunctionApp.Services;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.Azure.WebJobs;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Functions
{
    public class JobProfileSegmentRefresh
    {
        private const string MessageAction = "ActionType";
        private const string MessageContentType = "CType";
        private const string MessageContentId = "Id";
        private readonly string thisClassPath = typeof(JobProfileSegmentRefresh).FullName;

        private readonly ILogService logService;
        private readonly IMessageProcessor processor;
        private readonly ICorrelationIdProvider correlationIdProvider;

        public JobProfileSegmentRefresh(
            ILogService logService,
            IMessageProcessor processor,
            ICorrelationIdProvider correlationIdProvider)
        {
            this.logService = logService;
            this.processor = processor;
            this.correlationIdProvider = correlationIdProvider;
        }

        [FunctionName("JobProfileSegmentRefresh")]
        public async Task Run([ServiceBusTrigger("%job-profiles-refresh-topic%", "%job-profiles-refresh-subscription%", Connection = "service-bus-connection-string")] ServiceBusReceivedMessage segmentRefreshMessage)
        {
            if (segmentRefreshMessage is null)
            {
                throw new System.ArgumentNullException(nameof(segmentRefreshMessage));
            }

            if (processor is null)
            {
                throw new System.ArgumentNullException(nameof(processor));
            }

            if (correlationIdProvider != null)
            {
                correlationIdProvider.CorrelationId = segmentRefreshMessage.CorrelationId;
            }

            segmentRefreshMessage.ApplicationProperties.TryGetValue(MessageAction, out var messageAction); // Parse to enum values
            segmentRefreshMessage.ApplicationProperties.TryGetValue(MessageContentType, out var messageCtype);
            segmentRefreshMessage.ApplicationProperties.TryGetValue(MessageContentId, out var messageContentId);

            // loggger should allow setting up correlation id and should be picked up from message
            logService.LogInformation($"{thisClassPath}: Received message action {messageAction} for type {messageCtype} with Id: {messageContentId}: Correlation id {segmentRefreshMessage.CorrelationId}");

            var message = Encoding.UTF8.GetString(segmentRefreshMessage.Body);

            //Check whether we need to defer failed messages?
            await processor.ProcessSegmentRefresEventAsync(message, segmentRefreshMessage.SequenceNumber).ConfigureAwait(false);
        }
    }
}