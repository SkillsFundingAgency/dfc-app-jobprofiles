using DFC.App.JobProfile.Data.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.RelatedCareersModels
{
    public class RelatedCareersSegmentDataModel : ISegmentDataModel
    {
        public const string SegmentName = "RelatedCareers";

        public DateTime? LastReviewed { get; set; }

        public IEnumerable<RelatedCareerDataModel> RelatedCareers { get; set; }
    }
}
