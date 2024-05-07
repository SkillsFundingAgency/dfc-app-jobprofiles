using System;

namespace DFC.App.JobProfile.Data.Models.PatchModels
{
    public class PatchApprenticeshipFrameworksModel : BasePatchModel
    {
        public Guid SOCCodeClassificationId { get; set; }

        public string SocCode { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string JobProfileTitle { get; set; }
    }
}