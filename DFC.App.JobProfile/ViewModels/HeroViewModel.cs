using DFC.App.JobProfile.Data.Models;

namespace DFC.App.JobProfile.ViewModels
{
    public class HeroViewModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }

        public string PageLocation { get; set; }

        public JobProfileCachedOverview OverviewSegment { get; set; }
    }
}
