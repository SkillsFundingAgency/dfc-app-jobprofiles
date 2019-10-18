using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.Data.Models
{
    public class RefreshSegmentResult
    {
        public JobProfileSegment Segment { get; set; }

        public HtmlString Markup { get; set; }

        public string Json { get; set; }

        public DateTime RefreshedAt { get; set; }

        public long RefreshSequence { get; set; }
    }
}
