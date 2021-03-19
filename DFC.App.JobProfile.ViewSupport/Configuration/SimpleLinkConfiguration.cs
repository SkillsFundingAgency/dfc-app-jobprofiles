using DFC.App.Services.Common.Registration;

namespace DFC.App.JobProfile.ViewSupport.Configuration
{
    internal abstract class SimpleLinkConfiguration :
        IConfiguredSimpleLinks,
        IRequireConfigurationRegistration
    {
        public string URLFormat { get; set; }
    }
}