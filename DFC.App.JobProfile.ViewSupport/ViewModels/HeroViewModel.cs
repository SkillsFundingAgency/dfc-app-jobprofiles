using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewSupport.Models;

namespace DFC.App.JobProfile.ViewSupport.ViewModels
{
    public class HeroViewModel
    {
        public string Title { get; set; }

        public string AlternativeTitle { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }

        public string PageLocation { get; set; }

        public JobProfileOverview OverviewSegment { get; set; }

        public Breadcrumb Breadcrumb { get; set; }

        public string LabourMarketInformationLink { get; set; }
    }
}
