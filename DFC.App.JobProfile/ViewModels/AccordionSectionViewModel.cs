using Microsoft.AspNetCore.Html;

namespace DFC.App.JobProfile.ViewModels
{
    public class AccordionSectionViewModel
    {
        public int SequenceNo { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public HtmlString Content { get; set; }
    }
}
