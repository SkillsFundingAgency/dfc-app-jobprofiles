using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class OpportunityCourse
    {
        public Anchor Header { get; set; }

        public string Provider { get; set; }

        public string StartDate { get; set; }

        public string Location { get; set; }
    }
}
