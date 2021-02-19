using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileCareerPathModel
    {
        public string CareerPathAndProgression { get; set; }

        public IReadOnlyCollection<JobProfileApiContentItemModel> ApprecticeshipStandard { get; set; }
    }
}
