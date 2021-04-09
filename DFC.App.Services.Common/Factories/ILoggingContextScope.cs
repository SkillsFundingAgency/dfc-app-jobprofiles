using System;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Factories
{
    /// <summary>
    /// I logging context scope.
    /// </summary>
    public interface ILoggingContextScope :
        IDisposable
    {
        /// <summary>
        /// Log...
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>The currently running task.</returns>
        Task Log(string message);

        /// <summary>
        /// Log...
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The currently running task.</returns>
        Task Log(Exception exception);
    }
}