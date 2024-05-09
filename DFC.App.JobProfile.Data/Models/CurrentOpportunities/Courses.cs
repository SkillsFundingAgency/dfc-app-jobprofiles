using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Models.CurrentOpportunities
{
    public class Courses
    {
        public string CourseKeywords { get; set; }

        public IEnumerable<Opportunity> Opportunities { get; set; }
    }
}
