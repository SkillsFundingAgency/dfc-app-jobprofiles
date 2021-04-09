using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Helpers
{
    /// <summary>
    /// HTTP response message result.
    /// Can't be tested, too many un-implementable abstracts...
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class HttpResponseMessageResult : IActionResult
    {
        /// <summary>
        /// The http response message.
        /// </summary>
        private readonly HttpResponseMessage _responseMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseMessageResult"/> class.
        /// </summary>
        /// <param name="responseMessage">the wrapped response message.</param>
        public HttpResponseMessageResult(HttpResponseMessage responseMessage)
        {
            It.IsNull(responseMessage)
                .AsGuard<ArgumentNullException>();

            _responseMessage = responseMessage;
        }

        /// <summary>
        /// Get stream.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>The stream.</returns>
        public async Task<Stream> GetStream(HttpContent candidate) =>
            It.Has(candidate)
                ? await candidate.ReadAsStreamAsync()
                : new MemoryStream();

        /// <inheritdoc/>
        public async Task ExecuteResultAsync(ActionContext context)
        {
            It.IsNull(context)
                .AsGuard<ArgumentNullException>();

            context.HttpContext.Response.StatusCode = (int)_responseMessage.StatusCode;

            _responseMessage.Headers
                .ForEach(header =>
                {
                    context.HttpContext.Response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));
                });

            using (var stream = await GetStream(_responseMessage.Content))
            {
                await stream.CopyToAsync(context.HttpContext.Response.Body);
                await context.HttpContext.Response.Body.FlushAsync();
            }
        }
    }
}