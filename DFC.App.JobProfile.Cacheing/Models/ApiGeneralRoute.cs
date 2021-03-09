using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ApiGeneralRoute
    {
        public string Topic { get; set; }

        public IReadOnlyCollection<string> Descriptions { get; set; }
    }
}
