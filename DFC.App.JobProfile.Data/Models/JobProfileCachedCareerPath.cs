using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileCachedCareerPath
    {
        public string CareerPathAndProgression { get; set; }

        public IReadOnlyCollection<ContentApiBranchElement> ApprecticeshipStandard { get; set; }
    }
}
