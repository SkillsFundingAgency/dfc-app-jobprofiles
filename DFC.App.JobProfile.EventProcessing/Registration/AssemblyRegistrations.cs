using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.EventProcessing.Models;
using DFC.App.JobProfile.EventProcessing.Services;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;

// Project level
// configuration
//[assembly: ConfigurationRegistration(typeof(IEventGridSubscriptionConfiguration), typeof(EventGridSubscriptionConfiguration), nameof(EventGridPublicationConfiguration))]
[assembly: ConfigurationRegistration(typeof(IEventGridPublicationConfiguration), typeof(EventGridPublicationConfiguration), nameof(EventGridPublicationConfiguration))]

// services
[assembly: InternalRegistration(typeof(IEventMessageService<JobProfileCached>), typeof(EventMessageService<JobProfileCached>), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IEventMessageService<StaticItemCached>), typeof(EventMessageService<StaticItemCached>), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IEventGridService<JobProfileCached>), typeof(EventGridService<JobProfileCached>), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IEventGridClientService), typeof(EventGridClientService), TypeOfRegistrationScope.Singleton)]
