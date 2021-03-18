using DFC.App.JobProfile.Data.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class LmiLinkViewModel
    {
        public OverviewSegmentModel OverviewSegmentModel { get; set; }

        public string CanonicalName { get; set; }
    }
}
