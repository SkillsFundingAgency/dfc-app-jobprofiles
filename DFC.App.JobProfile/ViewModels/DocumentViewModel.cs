// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
using DFC.App.JobProfile.Cacheing.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.ViewModels
{
    public class DocumentViewModel
    {
        public HeadViewModel Head { get; set; } = new HeadViewModel();

        public BreadcrumbViewModel Breadcrumb { get; set; }

        public HeroViewModel HeroBanner { get; set; } = new HeroViewModel();

        public BodyViewModel Body { get; set; } = new BodyViewModel();

        //[Display(Name = "Document Id")]
        //public Guid? DocumentId { get; set; }

        [Display(Name = "Canonical Name")]
        public string CanonicalName { get; set; }

        [Display(Name = "Breadcrumb Title")]
        public string BreadcrumbTitle { get; set; }

        [Display(Name = "Include In SiteMap")]
        public bool IncludeInSitemap { get; set; }

        //[Display(Name = "Last Updated")]
        //public DateTime LastReviewed { get; set; }

        //public long SequenceNumber { get; set; }

        //[Display(Name = "Alternative Names")]
        //public string[] AlternativeNames { get; set; }

        //public string JobProfileWebsiteUrl { get; set; }

        //public string PageLocation { get; set; }

        //public string skos__prefLabel { get; set; }

        //public DateTime ModifiedDate { get; set; }

        //public DateTime CreatedDate { get; set; }

        //public string HtbCareerTips { get; set; }

        //public string WitDigitalSkillsLevel { get; set; }

        //public string TitleOptions { get; set; }

        //public string HtbFurtherInformation { get; set; }

        //public IList<ContentApiBranchElement> ContentItems { get; set; }

        //public List<StaticContentItemModel> SharedContent { get; set; }
    }
}