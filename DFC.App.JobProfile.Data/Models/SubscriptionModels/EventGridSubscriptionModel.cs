using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.Data.Models.SubscriptionModels
{
    public class EventGridSubscriptionModel
    {
        public string? Name { get; set; }

        public string? Endpoint { get; set; }

        public SubscriptionFilterModel Filter { get; set; } = new SubscriptionFilterModel();
    }
}
