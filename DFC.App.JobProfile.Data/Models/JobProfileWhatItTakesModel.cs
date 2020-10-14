using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileWhatItTakesModel
    {
        public List<JobProfileApiContentItemModel> OtherRequirement;

        public List<JobProfileApiContentItemModel> Restrictions { get; set; }

        public JobProfileApiContentItemModel Occupation { get; set; }
    }
}
