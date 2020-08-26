using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.ViewModels
{
    public class DocumentViewModel
    {
        public HeadViewModel Head { get; set; }

        public BreadcrumbViewModel Breadcrumb { get; set; }

        public BodyViewModel Body { get; set; }

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

        [Display(Name = "Last Updated")]
        public DateTime LastReviewed { get; set; }

        public long SequenceNumber { get; set; }

        [Display(Name = "Alternative Names")]
        public string[] AlternativeNames { get; set; }

        public string JobProfileWebsiteUrl { get; set; }

        public string PageLocation { get; set; }

        public string skos__prefLabel { get; set; }

        public DateTime ModifiedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? SalaryStarter { get; set; }

        public string? SalaryExperienced { get; set; }

        public int? MinimumHours { get; set; }

        public string HtbCareerTips { get; set; }

        public string HtbBodies { get; set; }

        public string WitDigitalSkillsLevel { get; set; }

        public string WorkingPattern { get; set; }

        public string WorkingHoursDetails { get; set; }

        public string TitleOptions { get; set; }

        public int? MaximumHours { get; set; }

        public string WorkingPatternDetails { get; set; }

        public string CareerPathAndProgression { get; set; }

        public string HtbFurtherInformation { get; set; }

        public IList<ApiContentItemModel> ContentItems { get; set; }

        public List<StaticContentItemModel> SharedContent { get; set; }
    }
}