﻿using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class SegmentVacancy
    {
        public string Title { get; set; }

        public SegmentLocation Location { get; set; }

        public string WageUnit { get; set; }

        public string WageText { get; set; }

        public string URL { get; set; }
    }
}
