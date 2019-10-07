using DFC.App.JobProfile.Data.Contracts;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.JobProfileSkillModels
{
    public class JobProfileSkillSegmentDataModel : ISegmentDataModel
    {
        public const string SegmentName = "WhatItTakes";

        public JobProfileSkillSegmentDataModel()
        {
            Skills = new List<JobProfileSkillSegmentSkillDataModel>();
            Restrictions = new List<GenericListContent>();
        }

        public DateTime LastReviewed { get; set; }

        public string Summary { get; set; }

        public IEnumerable<JobProfileSkillSegmentSkillDataModel> Skills { get; set; }

        public string DigitalSkill { get; set; }

        public string RestrictionsSummary { get; set; }

        public IEnumerable<GenericListContent> Restrictions { get; set; }

        public string OtherRequirements { get; set; }
    }
}
