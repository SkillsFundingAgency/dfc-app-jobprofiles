﻿using AutoMapper;
using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.PatchModels;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Services
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IMapper mapper;
        private readonly IHttpClientService<JobProfileMetadata> httpClientService;
        private readonly ILogger log;

        public MessageProcessor(IMapper mapper, IHttpClientService<JobProfileMetadata> httpClientService, ILogger log)
        {
            this.mapper = mapper;
            this.httpClientService = httpClientService;
            this.log = log;
        }

        public async Task<HttpStatusCode> ProcessSegmentRefresEventAsync(string eventData)
        {
            if (string.IsNullOrWhiteSpace(eventData))
            {
                throw new ArgumentException("Event data cannot be null or empty.", nameof(eventData));
            }

            var refreshPayload = JsonConvert.DeserializeObject<RefreshJobProfileSegment>(eventData);
            var result = await httpClientService.PostAsync(refreshPayload, "refresh").ConfigureAwait(false);

            if (result == HttpStatusCode.OK)
            {
                log.LogInformation($"{nameof(ProcessSegmentRefresEventAsync)}: Segment: {refreshPayload.Segment} of job profile: '{refreshPayload.CanonicalName} - {refreshPayload.JobProfileId}' updated.");
            }
            else
            {
                log.LogWarning($"{nameof(ProcessSegmentRefresEventAsync)}: Segment: {refreshPayload.Segment} of job profile: '{refreshPayload.CanonicalName} - {refreshPayload.JobProfileId}' NOT updated : Status: {result}");
            }

            return result;
        }

        public Task<HttpStatusCode> ProcessSitefinityMessageAsync(string message, string messageAction, string messageCtype, string messageContentId, long sequenceNumber)
        {
            if (string.IsNullOrWhiteSpace(messageCtype))
            {
                throw new ArgumentException("Message content type cannot be null or empty.", nameof(messageCtype));
            }

            if (!Enum.TryParse<MessageContentType>(messageCtype, out var contentType))
            {
                throw new ArgumentOutOfRangeException(nameof(messageCtype), $"Invalid message content type '{messageCtype}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageContentType)))}'");
            }

            switch (contentType)
            {
                case MessageContentType.JobProfile:
                    return ProcessJobProfileMessageAsync(message, messageAction, messageContentId, sequenceNumber);

                default:
                    break;
            }

            return Task.FromResult(HttpStatusCode.InternalServerError);
        }

        private async Task<HttpStatusCode> ProcessJobProfileMessageAsync(string message, string messageAction, string messageContentId, long sequenceNumber)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));
            }

            if (string.IsNullOrWhiteSpace(messageAction))
            {
                throw new ArgumentException("Message action cannot be null or empty.", nameof(messageAction));
            }

            if (string.IsNullOrWhiteSpace(messageContentId))
            {
                throw new ArgumentException("Message content id cannot be null or empty", nameof(messageContentId));
            }

            if (!Enum.TryParse<MessageAction>(messageAction, out var action))
            {
                throw new ArgumentOutOfRangeException(nameof(messageAction), $"Invalid message action '{messageAction}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageAction)))}'");
            }

            var jobProfileMessage = JsonConvert.DeserializeObject<JobProfileMessage>(message);
            var jobProfile = mapper.Map<JobProfileMetadata>(jobProfileMessage);
            jobProfile.SequenceNumber = sequenceNumber;

            switch (action)
            {
                case MessageAction.Draft:
                case MessageAction.Published:
                    var result = await httpClientService.PatchAsync(jobProfile).ConfigureAwait(false);
                    if (result == HttpStatusCode.NotFound)
                    {
                        return await httpClientService.PostAsync(jobProfile).ConfigureAwait(false);
                    }

                    return result;

                case MessageAction.Deleted:
                    return await httpClientService.DeleteAsync(jobProfile.JobProfileId).ConfigureAwait(false);

                default:
                    throw new ArgumentOutOfRangeException(nameof(messageAction), $"Invalid message action '{messageAction}' received, should be one of '{string.Join(",", Enum.GetNames(typeof(MessageAction)))}'");
            }
        }
    }
}