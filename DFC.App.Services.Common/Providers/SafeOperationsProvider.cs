using DFC.App.Services.Common.Registration;
using System;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Providers
{
    /// <summary>
    /// The safe operations provider (implementation).
    /// </summary>
    public sealed class SafeOperationsProvider :
        IProvideSafeOperations,
        IRequireServiceRegistration
    {
        /// <inheritdoc/>
        public async Task Try(Func<Task> doAction, Func<Exception, Task> handleError)
        {
            try
            {
                await doAction();
            }
            catch (Exception e)
            {
                await handleError?.Invoke(e);
            }
        }

        /// <inheritdoc/>
        public async Task<TResult> Try<TResult>(Func<Task<TResult>> doAction, Func<Exception, Task<TResult>> handleError)
        {
            try
            {
                return await doAction();
            }
            catch (Exception e)
            {
                return await handleError(e);
            }
        }
    }
}