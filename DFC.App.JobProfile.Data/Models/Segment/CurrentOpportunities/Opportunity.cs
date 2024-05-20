using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models.Segment.CurrentOpportunities
{
    [ExcludeFromCodeCoverage]
    public class Opportunity
    {
        public string Title { get; set; }

        public string CourseId { get; set; }

        public string RunId { get; set; }

        public string TLevelId { get; set; }

        public string TLevelLocationId { get; set; }

        public string Provider { get; set; }

        public string StartDate { get; set; }

        public bool FlexibleStartDate { get; set; }

        public Location Location { get; set; }

        public string Url { get; set; }

        public DateTime PullDate { get; set; } = DateTime.Now;
    }
}
