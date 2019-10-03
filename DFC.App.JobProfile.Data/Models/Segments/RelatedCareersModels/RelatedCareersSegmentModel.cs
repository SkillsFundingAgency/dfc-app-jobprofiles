using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.RelatedCareersModels
{
    public class RelatedCareersSegmentModel : BaseSegmentModel
    {
        public const string SegmentName = "RelatedCareers";

        public RelatedCareersSegmentDataModel Data { get; set; }
    }
}