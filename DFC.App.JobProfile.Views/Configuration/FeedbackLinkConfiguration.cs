using DFC.App.Services.Common.Registration;

namespace DFC.App.JobProfile.ViewSupport.Configuration
{
    internal sealed class FeedbackLinkConfiguration :
        IFeedbackLinks,
        IRequireConfigurationRegistration
    {
        public string SmartSurveyJP { get; set; }
    }
}