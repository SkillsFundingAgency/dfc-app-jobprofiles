using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles;
using System.Collections.Generic;
using RelatedSkill = DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles.RelatedSkill;

namespace DFC.App.JobProfile.ProfileService.Models
{
    public class JobProfileSkillsModel
    {
       public List<JobSkills> JobSkills { get; set; }
    }

    public class JobSkills
    {
        public Skills Skills { get; set; }

        public RelatedSkill JobProfileContextualisedSkills { get; set; }
    }
}
