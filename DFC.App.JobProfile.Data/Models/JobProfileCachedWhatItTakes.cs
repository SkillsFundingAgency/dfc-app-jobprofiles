using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileCachedWhatItTakes
    {
        public IReadOnlyCollection<ContentApiBranchElement> OtherRequirement { get; set; }

        public IReadOnlyCollection<ContentApiBranchElement> Restrictions { get; set; }

        public ContentApiBranchElement Occupation { get; set; }
    }
}
