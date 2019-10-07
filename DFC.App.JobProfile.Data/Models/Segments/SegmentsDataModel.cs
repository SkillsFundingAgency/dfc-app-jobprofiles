using DFC.App.JobProfile.Data.Models.Segments.CareerPathModels;
using DFC.App.JobProfile.Data.Models.Segments.CurrentOpportunitiesModels;
using DFC.App.JobProfile.Data.Models.Segments.HowToBecomeModels;
using DFC.App.JobProfile.Data.Models.Segments.JobProfileSkillModels;
using DFC.App.JobProfile.Data.Models.Segments.JobProfileTasksModels;
using DFC.App.JobProfile.Data.Models.Segments.OverviewBannerModels;
using DFC.App.JobProfile.Data.Models.Segments.RelatedCareersModels;

namespace DFC.App.JobProfile.Data.Models.Segments
{
    public class SegmentsDataModel
    {
        public CareerPathSegmentDataModel CareerPath { get; set; }

        public CurrentOpportunitiesSegmentDataModel CurrentOpportunities { get; set; }

        public HowToBecomeSegmentDataModel HowToBecome { get; set; }

        public OverviewBannerSegmentDataModel OverviewBanner { get; set; }

        public RelatedCareersSegmentDataModel RelatedCareers { get; set; }

        public JobProfileSkillSegmentDataModel WhatItTakes { get; set; }

        public JobProfileTasksSegmentDataModel WhatYouWillDo { get; set; }
    }
}
