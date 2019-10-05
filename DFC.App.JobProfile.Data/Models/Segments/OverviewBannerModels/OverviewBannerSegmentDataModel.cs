using DFC.App.JobProfile.Data.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.OverviewBannerModels
{
    public class OverviewBannerSegmentDataModel : ISegmentDataModel
    {
        public const string SegmentName = "OverviewBanner";

        public DateTime LastReviewed { get; set; }

        public string SocCode { get; set; }

        public string SocDescription { get; set; }

        public string Title { get; set; }

        public string AlternativeTitle { get; set; }

        public List<GenericListContent> HiddenAlternativeTitle { get; set; }

        public List<GenericListContent> JobProfileSpecialism { get; set; }

        public string Overview { get; set; }

        public int SalaryStarter { get; set; }

        public int SalaryExperienced { get; set; }

        public decimal MinimumHours { get; set; }

        public decimal MaximumHours { get; set; }

        public string WorkingHoursDetails { get; set; }

        public string WorkingPattern { get; set; }

        public string WorkingPatternDetails { get; set; }
    }
}
