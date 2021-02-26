﻿using DFC.App.JobProfile.ContentAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public class ContentApiRootElement :
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
        public JObject Curies { get; set; }

        [JsonProperty("skos__prefLabel")]
        public string Title { get; set; }

        public string TitleOptions { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }

        public string SalaryStarter { get; set; }

        public string SalaryExperienced { get; set; }

        public decimal MinimumHours { get; set; }

        public decimal MaximumHours { get; set; }

        public string CareerPathAndProgression { get; set; }

        public string WitDigitalSkillsLevel { get; set; }

        public string WorkingPattern { get; set; }

        public string WorkingHoursDetails { get; set; }

        public string WorkingPatternDetails { get; set; }

        public string HtbBodies { get; set; }

        public string HtbCareerTips { get; set; }

        public string HtbFurtherInformation { get; set; }

        public ContentApiWhatYoullDo WhatYoullDoSegment { get; set; }

        public ContentApiCareerPath CareerPathSegment { get; set; }

        public ContentApiHowToBecome HowToBecomeSegment { get; set; }

        public ContentApiWhatItTakes WhatItTakesSegment { get; set; }

        public ICollection<ContentApiBranchElement> ContentItems { get; set; } = new List<ContentApiBranchElement>();

        public bool IsFaultedState() =>
            Uri == UriExtra.Empty
            || Id == Guid.Empty
            || string.IsNullOrWhiteSpace(CanonicalName)
            || string.IsNullOrWhiteSpace(PageLocation);
    }
}