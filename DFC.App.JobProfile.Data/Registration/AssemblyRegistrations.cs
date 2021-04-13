using DFC.App.JobProfile.Data.Providers;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;

// Project level
// Providers
[assembly: InternalRegistration(typeof(IProvideJobProfiles), typeof(JobProfileProvider), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IProvideSharedContent), typeof(SharedContentProvider), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IProvideCurrentOpportunities), typeof(CurrentOpportunitiesProvider), TypeOfRegistrationScope.Singleton)]
