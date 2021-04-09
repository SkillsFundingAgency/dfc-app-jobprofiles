using DFC.App.Services.Common.Factories;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Providers
{
    /// <summary>
    /// I provide fault responses (contract).
    /// </summary>
    public interface IProvideFaultResponses
    {
        /// <summary>
        /// Get (the) response for...
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="loggingScope">The logging scope.</param>
        /// <returns>A http response message with a status and messge for the problem.</returns>
        Task<HttpResponseMessage> GetResponseFor(Exception exception, ILoggingContextScope loggingScope);
    }
}
