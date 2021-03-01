using DFC.App.JobProfile.ContentAPI.Configuration;
using DFC.App.JobProfile.ContentAPI.Services;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Services.ApiProcessorService;

// Project level
// Configuration
[assembly: ConfigurationRegistration(typeof(IContentApiConfiguration), typeof(ContentApiConfiguration), nameof(ContentApiConfiguration))]

// Services
[assembly: InternalRegistration(typeof(IProcessGraphCuries), typeof(GraphCurieProcessor), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IProvideGraphContent), typeof(GraphContentProvider), TypeOfRegistrationScope.Singleton)]

// registration dependencies
[assembly: ExternalRegistration(typeof(IApiService), typeof(ApiService), TypeOfRegistrationScope.Singleton)]
[assembly: ExternalRegistration(typeof(IApiDataProcessorService), typeof(ApiDataProcessorService), TypeOfRegistrationScope.Singleton)]
