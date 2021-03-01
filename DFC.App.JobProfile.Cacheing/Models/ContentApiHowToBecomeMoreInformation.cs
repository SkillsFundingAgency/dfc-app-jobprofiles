namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ContentApiHowToBecomeMoreInformation
    {
        public string CareerTips { get; set; }

        public string ProfessionalBodies { get; set; }

        public string FurtherInformation { get; set; }

        public bool HasItemsToDisplay() =>
            !string.IsNullOrWhiteSpace(CareerTips)
            || !string.IsNullOrWhiteSpace(ProfessionalBodies)
            || !string.IsNullOrWhiteSpace(FurtherInformation);
    }
}
