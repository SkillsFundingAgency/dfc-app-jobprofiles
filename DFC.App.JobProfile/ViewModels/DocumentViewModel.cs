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

        [Display(Name = "Last Reviewed")]
        public DateTime LastReviewed { get; set; }

        [Display(Name = "Alternative Names")]
        public string[] AlternativeNames { get; set; }

        [Display(Name = "Overview Banner Content")]
        public HtmlString OverviewBannerSegmentContent { get; set; }

        [Display(Name = "Overview Banner Last Reviewed")]
        public DateTime OverviewBannerSegmentLastReviewed { get; set; }

        [Display(Name = "Current Opportunities Content")]
        public HtmlString CurrentOpportunitiesSegmentContent { get; set; }

        [Display(Name = "Current Opportunities Last Reviewed")]
        public DateTime CurrentOpportunitiesSegmentLastReviewed { get; set; }

        [Display(Name = "Related Careers Content")]
        public HtmlString RelatedCareersSegmentContent { get; set; }

        [Display(Name = "Related Careers Last Reviewed")]
        public DateTime RelatedCareersSegmentLastReviewed { get; set; }

        [Display(Name = "Career Path Content")]
        public HtmlString CareerPathSegmentContent { get; set; }

        [Display(Name = "Career Path Last Reviewed")]
        public DateTime CareerPathSegmentLastReviewed { get; set; }

        [Display(Name = "How To Become Content")]
        public HtmlString HowToBecomeSegmentContent { get; set; }

        [Display(Name = "How To Become Last Reviewed")]
        public DateTime HowToBecomeSegmentLastReviewed { get; set; }

        [Display(Name = "What It Takes Content")]
        public HtmlString WhatItTakesSegmentContent { get; set; }

        [Display(Name = "What It Takes Last Reviewed")]
        public DateTime WhatItTakesSegmentLastReviewed { get; set; }
    }
}