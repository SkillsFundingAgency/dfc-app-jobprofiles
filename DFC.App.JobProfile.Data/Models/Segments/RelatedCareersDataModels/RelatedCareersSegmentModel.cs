using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.RelatedCareersDataModels
{
    public class RelatedCareersSegmentModel : BaseSegmentModel
    {
        public IEnumerable<RelatedCareerDataModel> RelatedCareers { get; set; }
    }
}