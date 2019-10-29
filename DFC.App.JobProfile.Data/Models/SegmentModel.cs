using DFC.App.JobProfile.Data.Enums;
using Microsoft.AspNetCore.Html;
using System;

namespace DFC.App.JobProfile.Data.Models
{
    public class SegmentModel
    {
        public JobProfileSegment Segment { get; set; }

        public HtmlString Markup { get; set; }

        public string Json { get; set; }

        public DateTime RefreshedAt { get; set; }

        public long RefreshSequence { get; set; }

        public RefreshStatus RefreshStatus { get; set; }
    }
}