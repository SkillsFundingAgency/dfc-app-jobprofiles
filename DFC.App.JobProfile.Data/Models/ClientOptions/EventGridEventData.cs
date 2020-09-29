using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.Data.Models.ClientOptions
{
    public class EventGridEventData
    {
        public string? Api { get; set; }

        public string? ItemId { get; set; }

        public string? VersionId { get; set; }

        public string? DisplayText { get; set; }

        public string? Author { get; set; }
    }
}
