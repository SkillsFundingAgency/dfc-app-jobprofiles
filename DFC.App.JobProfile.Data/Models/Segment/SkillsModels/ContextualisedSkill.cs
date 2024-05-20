using System;

namespace DFC.App.JobProfile.Data.Models.Segment.SkillsModels
{
    public class ContextualisedSkill
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string ONetAttributeType { get; set; }

        public decimal OriginalRank { get; set; }

        public decimal ONetRank { get; set; }
    }
}
