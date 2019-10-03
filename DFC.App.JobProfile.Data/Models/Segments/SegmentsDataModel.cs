using DFC.App.JobProfile.Data.Models.Segments.CareerPathModels;
using DFC.App.JobProfile.Data.Models.Segments.CurrentOpportunitiesModels;
using DFC.App.JobProfile.Data.Models.Segments.HowToBecomeModels;
using DFC.App.JobProfile.Data.Models.Segments.OverviewBannerModels;
using DFC.App.JobProfile.Data.Models.Segments.RelatedCareersModels;
using DFC.App.JobProfile.Data.Models.Segments.WhatItTakesModels;
using DFC.App.JobProfile.Data.Models.Segments.WhatYouWillDoModels;

namespace DFC.App.JobProfile.Data.Models.Segments
{
    public class SegmentsDataModel
    {
        public CareerPathSegmentDataModel CareerPath { get; set; }

        public CurrentOpportunitiesSegmentDataModel CurrentOpportunities { get; set; }

        public HowToBecomeSegmentDataModel HowToBecome { get; set; }

        public OverviewBannerSegmentDataModel OverviewBanner { get; set; }

        public RelatedCareersSegmentDataModel RelatedCareers { get; set; }

        public WhatItTakesSegmentDataModel WhatItTakes { get; set; }

        public WhatYouWillDoSegmentDataModel WhatYouWillDo { get; set; }
    }
}
