﻿using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Services
{
    public interface IMessageProcessor
    {
        Task<HttpStatusCode> ProcessSitefinityMessageAsync(string message, string messageAction, string messageCtype, string messageContentId, long sequenceNumber);

        Task<HttpStatusCode> ProcessSegmentRefresEventAsync(string eventData, long sequenceNumber);
    }
}
