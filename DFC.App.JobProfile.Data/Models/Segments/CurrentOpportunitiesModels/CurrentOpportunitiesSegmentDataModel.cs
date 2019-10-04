using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.CurrentOpportunitiesModels
{
    public class CurrentOpportunitiesSegmentDataModel
    {
        public string JobTitle { get; set; }

        public DateTime LastReviewed { get; set; }

        public IEnumerable<string> Standards { get; set; }

        public IEnumerable<string> Frameworks { get; set; }

        public string CourseKeywords { get; set; }

        public IEnumerable<Apprenticeship> Apprenticeships { get; set; }

        public IEnumerable<Course> Courses { get; set; }
    }
}
