using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class OpportunityApprenticeship
    {
        public Anchor Header { get; set; }

        public string Wage { get; set; }

        public string WageFrequency { get; set; }

        public string Location { get; set; }
    }
}
