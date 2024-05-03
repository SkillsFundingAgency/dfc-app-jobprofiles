using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Models.SkillsModels
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
