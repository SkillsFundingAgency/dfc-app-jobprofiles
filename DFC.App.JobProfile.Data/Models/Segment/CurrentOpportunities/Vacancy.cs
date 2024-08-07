﻿using System;

namespace DFC.App.JobProfile.Data.Models.Segment.CurrentOpportunities
{
    public class Vacancy
    {
        public string Title { get; set; }

        public string ApprenticeshipId { get; set; }

        public string WageUnit { get; set; }

        public string WageText { get; set; }

        public Location Location { get; set; }

        public Uri URL { get; set; }

        public DateTime PullDate { get; set; } = DateTime.UtcNow;
    }
}
