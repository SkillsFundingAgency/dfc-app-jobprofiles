using DFC.App.JobProfile.Cacheing.Services;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;
using Microsoft.Extensions.Hosting;

// Project level
// Services
[assembly: InternalRegistration(typeof(ILoadJobProfileContent), typeof(JobProfileCacheLoader), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(ILoadStaticContent), typeof(StaticContentLoader), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IMigrateCurrentOpportunities), typeof(MigratedOpportunitiesLoader), TypeOfRegistrationScope.Singleton)]

// Service hosts
[assembly: InternalRegistration(typeof(IHostedService), typeof(JobProfileBackgroundLoader), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IHostedService), typeof(StaticContentBackgroundLoader), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IHostedService), typeof(MigratedOpportunitiesBackgroundLoader), TypeOfRegistrationScope.Singleton)]
