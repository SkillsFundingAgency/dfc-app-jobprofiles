using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.Data.Models.Segments.RelatedCareersModels
{
    public class RelatedCareersSegmentDataModel
    {
        public DateTime? LastReviewed { get; set; }

        public IEnumerable<RelatedCareerDataModel> RelatedCareers { get; set; }
    }
}
