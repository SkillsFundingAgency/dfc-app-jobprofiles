using Microsoft.AspNetCore.Html;

namespace DFC.App.JobProfile.ViewModels
{
    public class SegmentsViewModel
    {
        public DefaultSegmentViewModel OverviewBanner { get; set; }

        public DefaultSegmentViewModel CurrentOpportunities { get; set; }

        public DefaultSegmentViewModel RelatedCareers { get; set; }

        public DefaultSegmentViewModel CareerPath { get; set; }

        public DefaultSegmentViewModel HowToBecome { get; set; }

        public DefaultSegmentViewModel WhatItTakes { get; set; }
    }
}
