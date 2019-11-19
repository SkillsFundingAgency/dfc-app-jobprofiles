using Microsoft.AspNetCore.Html;

namespace DFC.App.JobProfile.ViewModels
{
    public class SegmentsMarkupViewModel
    {
        public HtmlString OverviewBanner { get; set; }

        public HtmlString CurrentOpportunities { get; set; }

        public HtmlString RelatedCareers { get; set; }

        public HtmlString CareerPath { get; set; }

        public HtmlString HowToBecome { get; set; }

        public HtmlString WhatItTakes { get; set; }

        public HtmlString WhatYouWillDo { get; set; }
    }
}
