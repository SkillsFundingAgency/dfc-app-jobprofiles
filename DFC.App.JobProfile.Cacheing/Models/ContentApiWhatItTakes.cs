using System.Collections.Generic;

namespace DFC.App.JobProfile.Cacheing.Models
{
    public sealed class ContentApiWhatItTakes
    {
        public string Preface { get; set; } = "You'll need:";

        public IReadOnlyCollection<string> Skills { get; set; }

        public IReadOnlyCollection<string> Restrictions { get; set; }

        public IReadOnlyCollection<string> OtherRequirements { get; set; }
    }
}
