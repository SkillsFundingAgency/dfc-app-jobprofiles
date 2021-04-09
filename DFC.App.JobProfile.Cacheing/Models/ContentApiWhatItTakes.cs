using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Cacheing.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class ContentApiWhatItTakes
    {
        public string Preface { get; set; } = "You'll need:";

        public IReadOnlyCollection<string> Skills { get; set; }

        public IReadOnlyCollection<string> Restrictions { get; set; }

        public IReadOnlyCollection<string> OtherRequirements { get; set; }
    }
}
