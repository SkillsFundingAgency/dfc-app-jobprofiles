using DFC.App.JobProfile.Data.Models.Segments.CareerPathDataModels;
using DFC.App.JobProfile.Data.Models.Segments.CurrentOpportunitiesDataModels;
using DFC.App.JobProfile.Data.Models.Segments.HowToBecomeDataModels;
using DFC.App.JobProfile.Data.Models.Segments.OverviewBannerDataModels;
using DFC.App.JobProfile.Data.Models.Segments.RelatedCareersDataModels;
using DFC.App.JobProfile.Data.Models.Segments.WhatItTakesDataModels;
using DFC.App.JobProfile.Data.Models.Segments.WhatYouWillDoDataModels;

namespace DFC.App.JobProfile.Data.Models.Segments
{
    public class SegmentsDataModel
    {
        public CareerPathSegmentModel CareerPath { get; set; }

        public CurrentOpportunitiesSegmentModel CurrentOpportunities { get; set; }

        public HowToBecomeSegmentModel HowToBecome { get; set; }

        public OverviewBannerSegmentModel OverviewBanner { get; set; }

        public RelatedCareersSegmentModel RelatedCareers { get; set; }

        public WhatItTakesSegmentModel WhatItTakes { get; set; }

        public WhatYouWillDoSegmentModel WhatYouWillDo { get; set; }
    }
}
