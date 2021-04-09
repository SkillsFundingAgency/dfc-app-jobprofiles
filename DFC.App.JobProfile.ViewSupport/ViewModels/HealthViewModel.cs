using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ViewSupport.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class HealthViewModel
    {
        public List<HealthItemViewModel> HealthItems { get; set; }
    }
}
