using DFC.App.JobProfile.Data.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ViewModels
{

    [ExcludeFromCodeCoverage]

    public class HeroViewModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public JobProfileOverviewModel OverviewSegment { get; set; }

        public string JobProfileWebsiteUrl { get; set; }
    }
}
