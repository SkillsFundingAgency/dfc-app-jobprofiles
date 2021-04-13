using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class JobProfileOverview
    {
        public decimal MinimumHours { get; set; }

        public decimal MaximumHours { get; set; }

        public string SalaryStarter { get; set; }

        public string SalaryExperienced { get; set; }

        public string WorkingPattern { get; set; }

        public string WorkingHoursDetails { get; set; }

        public string WorkingPatternDetails { get; set; }
    }
}
