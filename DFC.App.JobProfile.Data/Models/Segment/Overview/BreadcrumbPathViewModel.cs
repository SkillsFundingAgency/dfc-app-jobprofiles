using System.Text.Json.Serialization;

namespace DFC.App.JobProfile.Data.Models.Overview
{
    public class BreadcrumbPathViewModel
    {
        public string Route { get; set; }

        public string Title { get; set; }

        [JsonIgnore]
        public bool AddHyperlink { get; set; } = true;
    }
}
