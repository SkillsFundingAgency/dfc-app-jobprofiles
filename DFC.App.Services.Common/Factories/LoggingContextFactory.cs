using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Factories
{
    /// <summary>
    /// The context logging factory (implementation).
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class LoggingContextFactory :
        ICreateLoggingContexts,
        IRequireServiceRegistration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingContextFactory"/> class.
        /// </summary>
        /// <param name="loggerProvider">the logger provider.</param>
        public LoggingContextFactory(ILoggerProvider loggerProvider)
        {
            It.IsNull(loggerProvider)
                .AsGuard<ArgumentNullException>();

            Factory = loggerProvider;
        }

        /// <summary>
        /// Gets the logger provider.
        /// </summary>
        internal ILoggerProvider Factory { get; }

        /// <inheritdoc/>
        public ILoggingContextScope BeginLoggingFor(string scope)
        {
            var logger = Factory.CreateLogger(scope);
            return new LoggingContextScope(logger, scope);
        }

        /// <summary>
        /// The context factory logging scope.
        /// The log routine on the logger is an extension method and therefore cannot be tested.
        /// This class implements the dispose pattern as prescribed by Sonar.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public class LoggingContextScope :
            ILoggingContextScope
        {
            /// <summary>
            /// the logging scope.
            /// </summary>
            private readonly string _scope;

            /// <summary>
            /// the scoped logger.
            /// </summary>
            private readonly ILogger _logger;

            /// <summary>
            /// Initializes a new instance of the <see cref="LoggingContextScope"/> class.
            /// </summary>
            /// <param name="logger">the logger.</param>
            /// <param name="scope">the scope.</param>
            public LoggingContextScope(ILogger logger, string scope)
            {
                _logger = logger;
                _scope = scope;
                _logger.Log(LogLevel.Information, $"commencing logging for: '{_scope}'", null);
            }

            /// <inheritdoc/>
            public Task Log(string message) =>
                Task.Run(() => _logger.Log(LogLevel.Information, message, null));

            /// <inheritdoc/>
            public Task Log(Exception exception) =>
                Task.Run(() => _logger.Log(LogLevel.Error, exception, null));

            /// <inheritdoc/>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Dispose...
            /// </summary>
            /// <param name="disposing">If disposing.</param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _logger.Log(LogLevel.Information, $"completed logging for: '{_scope}'", null);
                }
            }
        }
    }
}