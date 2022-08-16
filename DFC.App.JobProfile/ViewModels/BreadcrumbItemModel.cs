using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Models
{
    [ExcludeFromCodeCoverage]
    public class BreadcrumbItemModel
    {
        public string? Route { get; set; }

        public string? Title { get; set; }
    }
}
