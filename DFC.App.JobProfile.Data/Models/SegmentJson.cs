using System;

namespace DFC.App.JobProfile.Data.Models
{
    public class SegmentJson
    {
        public JobProfileSegment Segment { get; set; }

        public string Json { get; set; }

        public DateTime LastRefreshed { get; set; }

        public long RefreshSequenceNumber { get; set; }
    }
}