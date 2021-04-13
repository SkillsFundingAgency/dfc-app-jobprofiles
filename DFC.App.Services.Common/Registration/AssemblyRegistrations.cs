using DFC.App.Services.Common.Adapters;
using DFC.App.Services.Common.Converters;
using DFC.App.Services.Common.Factories;
using DFC.App.Services.Common.Faults;
using DFC.App.Services.Common.Providers;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;
using System.Net;

// Project level
// Adapters
[assembly: InternalRegistration(typeof(IAdaptActionResultOperations), typeof(ActionResultOperationsAdapter), TypeOfRegistrationScope.Singleton)]

// Converters
[assembly: InternalRegistration(typeof(IConvertJsonTypes), typeof(JsonTypeConverter), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IConvertXmlTypes), typeof(XmlTypeConverter), TypeOfRegistrationScope.Singleton)]

// Factories
[assembly: InternalRegistration(typeof(ICreateLoggingContexts), typeof(LoggingContextFactory), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(ICreateHttpResponseMessages), typeof(HttpResponseMessageFactory), TypeOfRegistrationScope.Singleton)]

// Providers
[assembly: InternalRegistration(typeof(IProvideAssets), typeof(AssetProvider), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IProvideSafeOperations), typeof(SafeOperationsProvider), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IProvideFaultResponses), typeof(FaultResponseProvider), TypeOfRegistrationScope.Singleton)]
[assembly: InternalRegistration(typeof(IProvideRegistrationDetails), typeof(RegistrationDetailProvider), TypeOfRegistrationScope.Singleton)]

// Faults
[assembly: FaultResponseRegistration(typeof(ConflictingResourceException), HttpStatusCode.Conflict)]
[assembly: FaultResponseRegistration(typeof(ResourceNotFoundException), HttpStatusCode.NotFound)]
[assembly: FaultResponseRegistration(typeof(MalformedRequestException), HttpStatusCode.BadRequest)]
[assembly: FaultResponseRegistration(typeof(NoContentException), HttpStatusCode.NoContent)]
[assembly: FaultResponseRegistration(typeof(UnprocessableEntityException), (HttpStatusCode)422)]
