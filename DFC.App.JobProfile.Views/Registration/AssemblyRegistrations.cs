using DFC.App.JobProfile.ViewSupport.Adapters;
using DFC.App.JobProfile.ViewSupport.Configuration;
using DFC.App.JobProfile.ViewSupport.Coordindators;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;

// Project level
// Configuration
[assembly: ConfigurationRegistration(typeof(IFeedbackLinks), typeof(FeedbackLinkConfiguration), nameof(FeedbackLinkConfiguration))]

// Adapters
[assembly: InternalRegistration(typeof(IAdaptProfileDocumentViews), typeof(ProfileDocumentViewsAdapter), TypeOfRegistrationScope.Singleton)]

// Converters
[assembly: InternalRegistration(typeof(ICoordinateProfileDocumentViews), typeof(ProfileDocumentViewsCoordinator), TypeOfRegistrationScope.Singleton)]
