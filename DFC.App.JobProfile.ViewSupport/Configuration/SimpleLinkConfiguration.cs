using DFC.App.Services.Common.Registration;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ViewSupport.Configuration
{
    [ExcludeFromCodeCoverage]
    internal abstract class SimpleLinkConfiguration :
        IConfiguredSimpleLinks,
        IRequireConfigurationRegistration
    {
        public string URLFormat { get; set; }
    }
}