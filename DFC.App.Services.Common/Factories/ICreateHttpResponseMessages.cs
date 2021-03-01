using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Factories
{
    /// <summary>
    /// I create http response messages.
    /// </summary>
    public interface ICreateHttpResponseMessages
    {
        /// <summary>
        /// Create...
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="responseCode">The response code.</param>
        /// <param name="responseContent">The response content.</param>
        /// <returns>The http response message.</returns>
        Task<HttpResponseMessage> Create<TContent>(HttpStatusCode responseCode, TContent responseContent);

        /// <summary>
        /// Create...
        /// </summary>
        /// <param name="responseCode">The response code.</param>
        /// <param name="responseContent">The response content.</param>
        /// <returns>The http response message.</returns>
        HttpResponseMessage Create(HttpStatusCode responseCode, string responseContent);
    }
}
