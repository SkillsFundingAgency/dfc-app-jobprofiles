using DFC.App.Services.Common.Factories;
using DFC.App.Services.Common.Faults;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Providers
{
    /// <summary>
    /// The fault response provider (implementation).
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class FaultResponseProvider :
        IProvideFaultResponses,
        IRequireServiceRegistration
    {
        private long _isLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultResponseProvider"/> class.
        /// </summary>
        /// <param name="details">The registration details provider.</param>
        /// <param name="response">The http response message factory.</param>
        public FaultResponseProvider(
            IProvideRegistrationDetails details,
            ICreateHttpResponseMessages response)
        {
            It.IsNull(details)
                .AsGuard<ArgumentNullException>(nameof(details));
            It.IsNull(response)
                .AsGuard<ArgumentNullException>(nameof(response));

            Details = details;
            Response = response;

            Map.Add(typeof(FallbackActionException), x => GenericResponse(HttpStatusCode.BadRequest, x.Message));
        }

        /// <summary>
        /// Gets or sets a value indicating whether is loaded.
        /// </summary>
        internal bool IsLoaded
        {
            get => Interlocked.Read(ref _isLoaded) == 1;
            set => Interlocked.Exchange(ref _isLoaded, value ? 1 : 0);
        }

        /// <summary>
        /// Gets the exception map.
        /// </summary>
        internal ExceptionMaps Map { get; } = new ExceptionMaps();

        /// <summary>
        /// Gets the registration provider details.
        /// </summary>
        internal IProvideRegistrationDetails Details { get; }

        /// <summary>
        /// Gets the http response message factory.
        /// </summary>
        internal ICreateHttpResponseMessages Response { get; }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetResponseFor(Exception exception, ILoggingContextScope loggingScope)
        {
            if (!IsLoaded)
            {
                Load();
            }

            var exceptionType = exception.GetType();

            if (Map.ContainsKey(exceptionType))
            {
                await InformOn(exception, loggingScope);
                var respondWith = Map[exceptionType];

                return respondWith(exception);
            }

            await loggingScope.Log(exception);

            var fallbackTo = Map[typeof(FallbackActionException)];

            return fallbackTo(exception);
        }

        internal void Load()
        {
            IsLoaded = true;
            var cache = Details.Load();
            cache.ForEach(LoadRegistrations);
        }

        internal void LoadRegistrations(Assembly registrant)
        {
            It.IsNull(registrant)
                .AsGuard<ArgumentNullException>(nameof(registrant));

            Details.GetAttributeListFor<FaultResponseRegistrationAttribute>(registrant)
                .ForEach(registration =>
                {
                    Map.Add(registration.ExceptionType, x => GenericResponse(registration.ResponseCode, x.Message));
                });
        }

        internal async Task InformOn(Exception exception, ILoggingContextScope loggingScope)
        {
            await loggingScope.Log(exception.Message);

            if (It.Has(exception.InnerException))
            {
                await InformOn(exception.InnerException, loggingScope);
            }
        }

        /// <summary>
        /// A generic registration response routine.
        /// </summary>
        /// <param name="code">The message return code.</param>
        /// <param name="message">The (optional) message.</param>
        /// <returns>A http response message.</returns>
        internal HttpResponseMessage GenericResponse(HttpStatusCode code, string message = "") =>
            Response.Create(code, message);

        /// <summary>
        /// Exception maps used inside the fault response provider.
        /// </summary>
        internal sealed class ExceptionMaps : Dictionary<Type, Func<Exception, HttpResponseMessage>>
        {
        }
    }
}