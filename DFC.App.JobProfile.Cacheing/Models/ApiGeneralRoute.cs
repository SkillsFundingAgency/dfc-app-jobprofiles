using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class ApiGeneralRoute
    {
        public string Topic { get; set; }

        public IReadOnlyCollection<string> Descriptions { get; set; }
    }
}
