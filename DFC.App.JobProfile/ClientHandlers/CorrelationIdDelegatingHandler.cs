// TODO: check this out!!! CorrelationId breaking changes...
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
// using CorrelationId;
using DFC.Logger.AppInsights.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace DFC.App.JobProfile.ClientHandlers
{
    [ExcludeFromCodeCoverage]
    public class CorrelationIdDelegatingHandler : DelegatingHandler
    {
        private readonly ICorrelationIdProvider correlationContextAccessor;
        private readonly ILogService logService;

        public CorrelationIdDelegatingHandler(
            ICorrelationIdProvider correlationContextAccessor,
            ILogService logService)
        {
            this.correlationContextAccessor = correlationContextAccessor;
            this.logService = logService;
        }

        //protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        //{
        //    if (correlationContextAccessor.CorrelationId != null &&
        //        request != null &&
        //        !request.Headers.Contains(correlationContextAccessor.CorrelationContext.Header))
        //    {
        //        request.Headers.Add(correlationContextAccessor.CorrelationContext.Header, correlationContextAccessor.CorrelationContext.CorrelationId);
        //        logService.LogInformation($"Added CorrelationID header with name {correlationContextAccessor.CorrelationContext.Header} and value {correlationContextAccessor.CorrelationContext.CorrelationId}");
        //    }

        //    return base.SendAsync(request, cancellationToken)
        //}
    }
}