﻿namespace DFC.App.JobProfile.Data.Models.ClientOptions
{
    public class EventGridSubscriptionClientOptions : ClientOptionsModel
    {
        public string Endpoint { get; set; } = "api/execute/";
    }
}