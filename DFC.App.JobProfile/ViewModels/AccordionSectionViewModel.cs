using Microsoft.AspNetCore.Html;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class AccordionSectionViewModel
    {
        public int SequenceNo { get; set; }

        public string Title { get; set; }

        public string AccordionID2 { get; set; }

        public string Summary { get; set; }

        public HtmlString Content { get; set; }
    }
}
