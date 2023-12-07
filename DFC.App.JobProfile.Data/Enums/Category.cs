using System.ComponentModel;

namespace DFC.App.JobProfile.Data.Enums
{
    public enum Category
    {
        [Description("None")]
        None,
        [Description("Careers advice and guidance")]
        AdviceGuidance,
        [Description("Courses, training and qualifications")]
        Courses,
        [Description("Problems using the website")]
        Website,
        [Description("Give feedback")]
        Feedback,
        [Description("Other")]
        Other,
    }
}
