namespace DFC.App.JobProfile.Data.Models
{
    public class HowToBecomeMoreInformation
    {
        public string Registration { get; set; }

        public string CareerTips { get; set; }

        public string ProfessionalBodies { get; set; }

        public string FurtherInformation { get; set; }

        public bool HasItemsToDisplay() =>
            !string.IsNullOrWhiteSpace(CareerTips)
            || !string.IsNullOrWhiteSpace(ProfessionalBodies)
            || !string.IsNullOrWhiteSpace(FurtherInformation);
    }
}
