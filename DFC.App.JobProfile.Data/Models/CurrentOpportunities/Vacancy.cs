﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace DFC.App.JobProfile.Data.Models.CurrentOpportunities
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
