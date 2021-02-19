using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileWhatItTakesModel
    {
        public IReadOnlyCollection<JobProfileApiContentItemModel> OtherRequirement { get; set; }

        public IReadOnlyCollection<JobProfileApiContentItemModel> Restrictions { get; set; }

        public JobProfileApiContentItemModel Occupation { get; set; }
    }
}
