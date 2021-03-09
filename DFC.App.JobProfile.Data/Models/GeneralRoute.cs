using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class GeneralRoute
    {
        public string Topic { get; set; }

        public IReadOnlyCollection<string> Descriptions { get; set; }
    }
}
