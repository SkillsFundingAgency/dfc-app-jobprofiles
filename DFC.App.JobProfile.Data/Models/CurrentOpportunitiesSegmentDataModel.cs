using System;

namespace DFC.App.JobProfile.Data.Models
{
    public class CurrentOpportunitiesSegmentDataModel
    {
        public const string SegmentName = "CurrentOpportunities";

        public DateTime LastReviewed { get; set; }

        public string JobTitle { get; set; }

        public string TitlePrefix { get; set; }

        public string ContentTitle { get; set; }

        public Apprenticeships Apprenticeships { get; set; }

        public Courses Courses { get; set; }
    }
}
