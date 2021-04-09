using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class GeneralRoute
    {
        public string Topic { get; set; }

        public IReadOnlyCollection<string> Descriptions { get; set; }
    }
}
