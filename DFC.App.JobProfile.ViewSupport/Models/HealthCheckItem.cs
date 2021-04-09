using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Models
{
    [ExcludeFromCodeCoverage]
    public class HealthCheckItem
    {
        public string Service { get; set; }

        public string Message { get; set; }
    }
}
