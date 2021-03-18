using DFC.App.JobProfile.Data.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class HeroViewModel
    {
        public IList<SegmentModel> Segments { get; set; }

        public LmiLinkViewModel LmiLink { get; set; }
    }
}
