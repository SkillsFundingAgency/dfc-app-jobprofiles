using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileWhatYoullDoModel
    {
        public List<JobProfileApiContentItemModel> DaytoDayTasks { get; set; }

        public List<JobProfileApiContentItemModel> WorkingEnvironment { get; set; }

        public JobProfileApiContentItemModel WorkingLocation { get; set; }

        public JobProfileApiContentItemModel WorkingUniform { get; set; }
    }
}
