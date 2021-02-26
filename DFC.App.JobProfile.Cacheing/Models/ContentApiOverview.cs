﻿namespace DFC.App.JobProfile.Cacheing.Models
{
    public class JobProfileCachedOverview
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
