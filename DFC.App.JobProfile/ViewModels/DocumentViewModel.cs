using Microsoft.AspNetCore.Html;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.ViewModels
{
    public class DocumentViewModel
    {
        public BreadcrumbViewModel Breadcrumb { get; set; }

        [Display(Name = "Document Id")]
        public Guid? DocumentId { get; set; }

        [Display(Name = "Canonical Name")]
        public string CanonicalName { get; set; }

        [Display(Name = "Breadcrumb Title")]
        public string BreadcrumbTitle { get; set; }

        [Display(Name = "Include In SiteMap")]
        public bool IncludeInSitemap { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }

        public DateTime LastReviewed { get; set; }

        [Display(Name = "Alternative Names")]
        public string[] AlternativeNames { get; set; }

        [Display(Name = "Overview Banner Markup")]
        public HtmlString OverviewBannerSegmentMarkup { get; set; }

        [Display(Name = "Overview Banner Updated")]
        public DateTime OverviewBannerSegmentUpdated { get; set; }

        [Display(Name = "Current Opportunities Markup")]
        public HtmlString CurrentOpportunitiesSegmentMarkup { get; set; }

        [Display(Name = "Current Opportunities Updated")]
        public DateTime CurrentOpportunitiesSegmentUpdated { get; set; }

        [Display(Name = "Related Careers Markup")]
        public HtmlString RelatedCareersSegmentMarkup { get; set; }

        [Display(Name = "Related Careers Updated")]
        public DateTime RelatedCareersSegmentUpdated { get; set; }

        [Display(Name = "Career Path Markup")]
        public HtmlString CareerPathSegmentMarkup { get; set; }

        [Display(Name = "Career Path Updated")]
        public DateTime CareerPathSegmentUpdated { get; set; }

        [Display(Name = "How To Become Markup")]
        public HtmlString HowToBecomeSegmentMarkup { get; set; }

        [Display(Name = "How To Become Updated")]
        public DateTime HowToBecomeSegmentUpdated { get; set; }

        [Display(Name = "What It Takes Markup")]
        public HtmlString WhatItTakesSegmentMarkup { get; set; }

        [Display(Name = "What It Takes Updated")]
        public DateTime WhatItTakesSegmentUpdated { get; set; }

        [Display(Name = "What You Will Do Markup")]
        public HtmlString WhatYouWillDoSegmentMarkup { get; set; }

        [Display(Name = "What You Will Do Updated")]
        public DateTime WhatYouWillDoSegmentUpdated { get; set; }
    }
}