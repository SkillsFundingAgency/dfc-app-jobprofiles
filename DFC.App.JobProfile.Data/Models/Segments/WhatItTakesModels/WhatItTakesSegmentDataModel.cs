using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.Data.Models.Segments.WhatItTakesModels
{
    public class WhatItTakesSegmentDataModel
    {
        public WhatItTakesSegmentDataModel()
        {
            Skills = new List<JobProfileSkillSegmentSkillDataModel>();
            Restrictions = new List<string>();
        }

        public DateTime? LastReviewed { get; set; }

        public string Summary { get; set; }

        public IEnumerable<JobProfileSkillSegmentSkillDataModel> Skills { get; set; }

        public string DigitalSkill { get; set; }

        public string RestrictionsSummary { get; set; }

        public IEnumerable<string> Restrictions { get; set; }

        public string OtherRequirements { get; set; }
    }
}
