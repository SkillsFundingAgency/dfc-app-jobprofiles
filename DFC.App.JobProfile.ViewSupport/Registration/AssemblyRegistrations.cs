using DFC.App.JobProfile.ViewSupport.Adapters;
using DFC.App.JobProfile.ViewSupport.Configuration;
using DFC.App.JobProfile.ViewSupport.Coordindators;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;

// Project level
// Configuration
[assembly: ConfigurationRegistration(typeof(IConfiguredFeedbackLinks), typeof(FeedbackLinkConfiguration), nameof(FeedbackLinkConfiguration))]
[assembly: ConfigurationRegistration(typeof(IConfiguredLabourMarketLinks), typeof(LabourMarketLinkConfiguration), nameof(LabourMarketLinkConfiguration))]

// Adapters
[assembly: InternalRegistration(typeof(IAdaptProfileDocumentViews), typeof(ProfileDocumentViewsAdapter), TypeOfRegistrationScope.Singleton)]

// Converters
[assembly: InternalRegistration(typeof(ICoordinateProfileDocumentViews), typeof(ProfileDocumentViewsCoordinator), TypeOfRegistrationScope.Singleton)]
