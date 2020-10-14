namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileHowToBecomeModel
    {
        public string Title { get; set; }

        public string HtbBodies { get; set; }

        public string HtbFurtherInformation { get; set; }

        public JobProfileApiContentItemModel DirectRoute { get; set; }

        public JobProfileApiContentItemModel ApprenticeshipRoute { get; set; }

        public JobProfileApiContentItemModel CollegeRoute { get; set; }

        public JobProfileApiContentItemModel OtherRoute { get; set; }

        public JobProfileApiContentItemModel Registration { get; set; }

        public JobProfileApiContentItemModel UniversityRoute { get; set; }

        public JobProfileApiContentItemModel VolunteeringRoute { get; set; }

        public JobProfileApiContentItemModel WorkRoute { get; set; }
    }
}
