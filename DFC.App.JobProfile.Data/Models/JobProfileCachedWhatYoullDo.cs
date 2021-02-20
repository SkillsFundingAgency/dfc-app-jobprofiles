using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileCachedWhatYoullDo
    {
        public List<ContentApiBranchElement> DaytoDayTasks { get; set; }

        public List<ContentApiBranchElement> WorkingEnvironment { get; set; }

        public ContentApiBranchElement WorkingLocation { get; set; }

        public ContentApiBranchElement WorkingUniform { get; set; }
    }
}
