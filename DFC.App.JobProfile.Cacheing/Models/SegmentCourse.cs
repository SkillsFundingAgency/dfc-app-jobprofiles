using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class SegmentCourse
    {
        public string Title { get; set; }

        public string Town { get; set; }

        public string Provider { get; set; }

        public DateTime StartDate { get; set; }

        public string URL { get; set; }
    }
}
