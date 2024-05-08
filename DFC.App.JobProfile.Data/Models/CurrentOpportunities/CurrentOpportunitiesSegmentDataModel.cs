using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Models.CurrentOpportunities
{
    public class CurrentOpportunitiesSegmentDataModel
    {
        public const string SegmentName = "CurrentOpportunities";

        public DateTime LastReviewed { get; set; }

        public string JobTitle { get; set; }

        public string TitlePrefix { get; set; }

        public string ContentTitle { get; set; }

        public Apprenticeships Apprenticeships { get; set; }

        public Courses Courses { get; set; }
    }
}
