namespace DFC.App.Services.Common.Factories
{
    /// <summary>
    /// i create logging contexts.
    /// </summary>
    public interface ICreateLoggingContexts
    {
        /// <summary>
        /// begin logging for...
        /// </summary>
        /// <param name="scope">the scope.</param>
        /// <returns>a logging scope.</returns>
        ILoggingContextScope BeginLoggingFor(string scope);
    }
}