using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segment.RelatedCareers
{
    public class RelatedCareerSegmentDataModel
    {
        public const string SegmentName = "RelatedCareers";

        public DateTime LastReviewed { get; set; }

        public List<RelatedCareerDataModel> RelatedCareers { get; set; }
    }
}
