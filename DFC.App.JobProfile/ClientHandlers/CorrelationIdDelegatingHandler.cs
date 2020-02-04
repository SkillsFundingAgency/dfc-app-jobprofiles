using CorrelationId;
using DFC.Logger.AppInsights.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ClientHandlers
{
    [ExcludeFromCodeCoverage]
    public class CorrelationIdDelegatingHandler : DelegatingHandler
    {
        private readonly ICorrelationContextAccessor correlationContextAccessor;
        private readonly ILogService logService;

        public CorrelationIdDelegatingHandler(
            ICorrelationContextAccessor correlationContextAccessor,
            ILogService logService)
        {
            this.correlationContextAccessor = correlationContextAccessor;
            this.logService = logService;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (this.correlationContextAccessor.CorrelationContext != null &&
                request != null &&
                !request.Headers.Contains(correlationContextAccessor.CorrelationContext.Header))
            {
                request.Headers.Add(correlationContextAccessor.CorrelationContext.Header, correlationContextAccessor.CorrelationContext.CorrelationId);
                logService.LogInformation($"Added CorrelationID header with name {correlationContextAccessor.CorrelationContext.Header} and value {correlationContextAccessor.CorrelationContext.CorrelationId}");
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}