using DFC.App.JobProfile.Data.Models;

namespace DFC.App.JobProfile.ViewModels
{
    public class HeroViewModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public JobProfileOverviewModel OverviewSegment { get; set; }

        public string JobProfileWebsiteUrl { get; set; }
    }
}
