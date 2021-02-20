// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
//using DFC.App.JobProfile.Data.Contracts;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace DFC.App.JobProfile.Data.Models
//{
//    public class JobProfileModel : BaseJobProfile, IDataModel
//    {
//        [Required]
//        [JsonProperty(PropertyName = "id")]
//        public Guid DocumentId
//        {
//            get => JobProfileId;
//            set => JobProfileId = value;
//        }

//        [Required]
//        public string SocLevelTwo { get; set; }

//        public string PartitionKey => SocLevelTwo.ToString();

//        [Required]
//        public DateTime LastReviewed { get; set; }

//        [JsonProperty(PropertyName = "_etag")]
//        public string Etag { get; set; }

//        public string BreadcrumbTitle { get; set; }

//        [Required]
//        public bool IncludeInSitemap { get; set; }

//        public IList<string> AlternativeNames { get; set; }

//        [Required]
//        public MetaTags MetaTags { get; set; }

//        public IList<SegmentModel> Segments { get; set; }

//        public IList<ContentApiBranchElement> ContentItems { get; set; } = new List<ContentApiBranchElement>();

//        public string JobProfileWebsiteUrl { get; set; }

//        public string PageLocation { get; set; }

//        public string skos__prefLabel { get; set; }

//        public string Description { get; set; }

//        public DateTime ModifiedDate { get; set; }

//        public DateTime CreatedDate { get; set; }

//        public string HtbCareerTips { get; set; }

//        public string WitDigitalSkillsLevel { get; set; }

//        public string TitleOptions { get; set; }

//        public List<StaticContentItemModel> SharedContent { get; set; }

//        public JobProfileOverviewModel OverviewSegment { get; set; }

//        public JobProfileWhatYoullDoModel WhatYoullDoSegment { get; set; }

//        public JobProfileCareerPathModel CareerPathSegment { get; set; }

//        public JobProfileHowToBecomeModel HowToBecomeSegment { get; set; }

//        public JobProfileWhatItTakesModel WhatItTakesSegment { get; set; }
//    }
//}