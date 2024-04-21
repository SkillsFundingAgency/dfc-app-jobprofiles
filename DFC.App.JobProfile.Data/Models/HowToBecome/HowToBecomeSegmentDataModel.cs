using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.HowToBecome
{
    public class HowToBecomeSegmentDataModel
    {
        public string Title { get; set; }

        public DateTime LastReviewed { get; set; }

        public string EntryRouteSummary { get; set; }

        public EntryRoutes EntryRoutes { get; set; }

        public MoreInformation MoreInformation { get; set; }

        public IList<Registration> Registrations { get; set; }

        public RealStory RealStory { get; set; }
    }
}