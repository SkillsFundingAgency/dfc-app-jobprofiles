using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segment.HowToBecome
{
    public class HowToBecomeSegmentDataModel
    {
        public string Title { get; set; }

        public DateTime LastReviewed { get; set; }

        public string EntryRouteSummary { get; set; }

        public EntryRoutes EntryRoutes { get; set; }

        public MoreInformation MoreInformation { get; set; }

        public List<Registration> Registrations { get; set; }

        public RealStory RealStory { get; set; }
    }
}