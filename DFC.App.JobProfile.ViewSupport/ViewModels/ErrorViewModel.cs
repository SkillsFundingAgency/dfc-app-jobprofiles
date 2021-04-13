using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ViewSupport.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}