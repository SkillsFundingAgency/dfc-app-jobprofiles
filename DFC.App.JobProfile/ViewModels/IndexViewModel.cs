using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ViewModels
{
    [ExcludeFromCodeCoverage]

    public class IndexViewModel
    {
        public IEnumerable<IndexDocumentViewModel> Documents { get; set; }
    }
}
