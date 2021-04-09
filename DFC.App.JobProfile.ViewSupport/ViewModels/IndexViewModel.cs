using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ViewSupport.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class IndexViewModel
    {
        public IReadOnlyCollection<OccupationSummaryViewModel> OccupationSummaries { get; set; }
    }
}
