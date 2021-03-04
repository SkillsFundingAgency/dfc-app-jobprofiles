using DFC.App.Services.Common.Converters;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Factories
{
    /// <summary>
    /// The http response message factory (implementation).
    /// </summary>
    public sealed class HttpResponseMessageFactory :
        ICreateHttpResponseMessages,
        IRequireServiceRegistration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseMessageFactory"/> class.
        /// </summary>
        /// <param name="converter">The json type converter.</param>
        public HttpResponseMessageFactory(IConvertJsonTypes converter)
        {
            It.IsNull(converter)
                .AsGuard<ArgumentNullException>(nameof(converter));

            Convert = converter;
        }

        /// <summary>
        /// Gets the json converter.
        /// </summary>
        internal IConvertJsonTypes Convert { get; }

        /// <inheritdoc/>
        public Task<HttpResponseMessage> Create<TContent>(HttpStatusCode responseCode, TContent responseContent) =>
            Task.Run(() =>
            {
                var newContent = Convert.ToString(responseContent);
                return Create(responseCode, newContent);
            });

        /// <inheritdoc/>
        public HttpResponseMessage Create(HttpStatusCode responseCode, string responseContent) =>
            new HttpResponseMessage(responseCode).SetContent(responseContent);
    }
}
