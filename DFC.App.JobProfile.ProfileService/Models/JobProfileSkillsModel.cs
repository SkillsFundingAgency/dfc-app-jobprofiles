using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
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
