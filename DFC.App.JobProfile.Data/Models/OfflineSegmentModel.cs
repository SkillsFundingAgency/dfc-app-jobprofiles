using Microsoft.AspNetCore.Html;

namespace DFC.App.JobProfile.Data.Models
{
    public class OfflineSegmentModel
    {
        public JobProfileSegment Segment { get; set; }

        public HtmlString OfflineMarkup { get; set; }

        public string OfflineJson { get; set; }
    }
}