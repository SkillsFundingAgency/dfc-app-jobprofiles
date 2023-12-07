using DFC.App.JobProfile.Data.Enums;

namespace DFC.App.JobProfile.Models
{
    public class SessionDataModel
    {
        public Category Category { get; set; }

        public string? MoreDetail { get; set; }

        public bool IsCallback { get; set; }
    }
}
