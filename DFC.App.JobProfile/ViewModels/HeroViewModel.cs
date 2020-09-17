using DFC.App.JobProfile.Data.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ViewModels
{

    [ExcludeFromCodeCoverage]

    public class HeroViewModel
    {
        //public IList<SegmentModel> Segments { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string? SalaryStarter { get; set; }

        public string? SalaryExperienced { get; set; }

        public int? MinimumHours { get; set; }

        public string WorkingPattern { get; set; }

        public int? MaximumHours { get; set; }

        public string WorkingHoursDetails { get; set; }

        public string WorkingPatternDetails { get; set; }
    }
}
