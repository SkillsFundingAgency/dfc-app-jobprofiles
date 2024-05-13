namespace DFC.App.JobProfile.Data.Models.Apprenticeships
{
    public class Wage
    {
        public double? WageAmount { get; set; }

        public double? WageAmountLowerBound { get; set; }

        public double? WageAmountUpperBound { get; set; }

        public string WageAdditionalInformation { get; set; }

        public string WageType { get; set; }

        public string WorkingWeekDescription { get; set; }

        public string WageUnit { get; set; }
    }
}