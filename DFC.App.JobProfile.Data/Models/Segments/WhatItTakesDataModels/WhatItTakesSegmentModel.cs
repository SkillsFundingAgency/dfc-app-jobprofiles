using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.WhatItTakesDataModels
{
    public class WhatItTakesSegmentModel : BaseSegmentModel
    {
        public WhatItTakesSegmentModel()
        {
            Skills = new List<JobProfileSkillSegmentSkillDataModel>();
            Restrictions = new List<string>();
        }

        public string Summary { get; set; }

        public IEnumerable<JobProfileSkillSegmentSkillDataModel> Skills { get; set; }

        public string DigitalSkill { get; set; }

        public string RestrictionsSummary { get; set; }

        public IEnumerable<string> Restrictions { get; set; }

        public string OtherRequirements { get; set; }
    }
}
