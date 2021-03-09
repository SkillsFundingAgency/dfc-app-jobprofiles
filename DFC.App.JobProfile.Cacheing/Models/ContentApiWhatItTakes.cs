using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ContentApiWhatItTakes
    {
        public IReadOnlyCollection<string> Restrictions { get; set; }

        public IReadOnlyCollection<string> OtherRequirements { get; set; }

        public IReadOnlyCollection<string> Skills { get; set; }
    }
}
