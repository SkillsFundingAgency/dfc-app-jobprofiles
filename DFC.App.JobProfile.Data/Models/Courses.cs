using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class Courses
    {
        public string CourseKeywords { get; set; }

        public IEnumerable<Opportunity> Opportunities { get; set; }
    }
}
