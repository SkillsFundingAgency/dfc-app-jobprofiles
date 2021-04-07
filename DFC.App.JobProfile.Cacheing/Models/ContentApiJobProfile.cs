using DFC.App.JobProfile.ContentAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ContentApiJobProfile :
        IRootContentItem<ContentApiBranchElement>
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty(PropertyName = "uri")]
        public Uri Uri { get; set; } = UriExtra.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.MinValue;

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; } = DateTime.MinValue;

        [JsonProperty("pagelocation_UrlName")]
        public string CanonicalName { get; set; }

        [JsonProperty("pagelocation_FullUrl")]
        public string PageLocation { get; set; }

        [JsonProperty("_links")]
        public JObject ContentLinks { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string Title { get; set; }

        public string AlternativeTitle { get; set; }

        public string Keywords { get; set; }

        #region job profile overview mapping

        public string SocCode { get; set; }

        public decimal MinimumHours { get; set; }

        public decimal MaximumHours { get; set; }

        public string SalaryStarter { get; set; }

        public string SalaryExperienced { get; set; }

        public string WorkingPattern { get; set; }

        public string WorkingHoursDetails { get; set; }

        public string WorkingPatternDetails { get; set; }

        #endregion job profile overview mapping

        public string Description { get; set; }

        public string CareerPathAndProgression { get; set; }

        [JsonProperty("WitDigitalSkillsLevel")]
        public string WhatItTakesDigitalSkillsLevel { get; set; }

        [JsonProperty("WydDayToDayTasks")]
        public string WhatYouWillDoDayToDayTasks { get; set; }

        [JsonProperty("HtbProfessionalBodies")]
        public string HowToBecomeProfessionalBodies { get; set; }

        [JsonProperty("HtbCareerTips")]
        public string HowToBecomeCareerTips { get; set; }

        [JsonProperty("HtbFurtherInformation")]
        public string HowToBecomeFurtherInformation { get; set; }

        public ContentApiWhatYouWillDo WhatYouWillDo { get; set; }

        public ContentApiCareerPath CareerPath { get; set; }

        public ContentApiHowToBecome HowToBecome { get; set; }

        public ContentApiWhatItTakes WhatItTakes { get; set; }

        public IReadOnlyCollection<ApiAnchor> RelatedCareers { get; set; }

        public ICollection<ContentApiBranchElement> ContentItems { get; set; } = new List<ContentApiBranchElement>();

        public bool IsFaultedState() =>
            Uri == UriExtra.Empty
            || Id == Guid.Empty
            || string.IsNullOrWhiteSpace(CanonicalName)
            || string.IsNullOrWhiteSpace(PageLocation);
    }
}