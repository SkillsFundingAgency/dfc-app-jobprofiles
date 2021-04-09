using DFC.App.JobProfile.Webhooks.Services;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;

// Project level
// Providers
[assembly: InternalRegistration(typeof(IProvideWebhooks), typeof(WebhooksProvider), TypeOfRegistrationScope.Singleton)]
