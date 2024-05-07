using DFC.App.JobProfile.Data.Models.CurrentOpportunities;

namespace DFC.App.JobProfile.Data.Models.PatchModels
{
    public class CurrentOpportunitiesPatchSegmentModel
    {
        public string CanonicalName { get; set; }

        public string SocLevelTwo { get; set; }

        public CurrentOpportunitiesSegmentDataModel Data { get; set; }
    }
}
