using System;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Providers
{
    /// <summary>
    /// I provide safe operations (contract).
    /// </summary>
    public interface IProvideSafeOperations
    {
        /// <summary>
        /// Try...
        /// </summary>
        /// <param name="doAction">Do action.</param>
        /// <param name="handleError">Handle error (can be null, optional).</param>
        /// <returns>The currently running task.</returns>
        Task Try(Func<Task> doAction, Func<Exception, Task> handleError);

        /// <summary>
        /// Try...
        /// </summary>
        /// <typeparam name="TResult">The return type for the action or the error.</typeparam>
        /// <param name="doAction">Do action.</param>
        /// <param name="handleError">Handle error.</param>
        /// <returns>The currently running task with the requested result type.</returns>
        Task<TResult> Try<TResult>(Func<Task<TResult>> doAction, Func<Exception, Task<TResult>> handleError);
    }
}