using System;

namespace DFC.App.JobProfile.Data.Models.Overview
{
    public class OverviewApiModel
    {
        public string Title { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public string Url { get; set; }

        public string Soc { get; set; }

        public string Soc2020 { get; set; }

        public string Soc2020Extension { get; set; }

        public string ONetOccupationalCode { get; set; }

        public string AlternativeTitle { get; set; }

        public string Overview { get; set; }

        public decimal? SalaryStarter { get; set; }

        public decimal? SalaryExperienced { get; set; }

        public decimal? MinimumHours { get; set; }

        public decimal? MaximumHours { get; set; }

        public string WorkingHoursDetailTitle { get; set; }

        public string WorkingPatternTitle { get; set; }

        public string WorkingPatternDetailTitle { get; set; }

        public BreadcrumbViewModel Breadcrumb { get; set; }
    }
}
