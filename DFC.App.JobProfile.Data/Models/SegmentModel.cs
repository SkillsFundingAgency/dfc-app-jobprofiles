using DFC.App.JobProfile.Data.Enums;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace DFC.App.JobProfile.Data.Models
{
    public class SegmentModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public JobProfileSegment Segment { get; set; }

        public HtmlString Markup { get; set; }

        public string JsonV1 { get; set; }

        [JsonIgnore]
        public string Json { get => JsonV1; set => JsonV1 = value; }

        public DateTime RefreshedAt { get; set; }

        public long RefreshSequence { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RefreshStatus RefreshStatus { get; set; }
    }
}