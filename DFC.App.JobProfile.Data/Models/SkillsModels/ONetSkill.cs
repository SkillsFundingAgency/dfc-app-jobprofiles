using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Models.SkillsModels
{
    public class OnetSkill
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string ONetElementId { get; set; }

        public string Description { get; set; }
    }
}
