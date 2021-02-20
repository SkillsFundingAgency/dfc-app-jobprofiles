namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileCachedHowToBecome
    {
        public string Title { get; set; }

        public string HtbBodies { get; set; }

        public string HtbFurtherInformation { get; set; }

        public ContentApiBranchElement DirectRoute { get; set; }

        public ContentApiBranchElement ApprenticeshipRoute { get; set; }

        public ContentApiBranchElement CollegeRoute { get; set; }

        public ContentApiBranchElement OtherRoute { get; set; }

        public ContentApiBranchElement Registration { get; set; }

        public ContentApiBranchElement UniversityRoute { get; set; }

        public ContentApiBranchElement VolunteeringRoute { get; set; }

        public ContentApiBranchElement WorkRoute { get; set; }
    }
}
