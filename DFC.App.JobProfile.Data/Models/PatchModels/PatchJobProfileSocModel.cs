using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.PatchModels
{
    public class PatchJobProfileSocModel : BasePatchModel
    {
        public string SocCode { get; set; }

        public IEnumerable<ApprenticeshipFramework> ApprenticeshipFramework { get; set; }

        public IEnumerable<ApprenticeshipStandard> ApprenticeshipStandards { get; set; }
    }
}