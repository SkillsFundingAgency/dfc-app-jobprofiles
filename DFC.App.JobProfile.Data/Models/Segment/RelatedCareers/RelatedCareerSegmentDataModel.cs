using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Models.RelatedCareersModels
{
    public class RelatedCareerSegmentDataModel
    {
        public const string SegmentName = "RelatedCareers";

        public DateTime LastReviewed { get; set; }

        public List<RelatedCareerDataModel> RelatedCareers { get; set; }
    }
}
