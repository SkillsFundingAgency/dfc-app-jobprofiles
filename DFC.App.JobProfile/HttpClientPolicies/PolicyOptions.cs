using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.HttpClientPolicies
{
    [ExcludeFromCodeCoverage]
    public class PolicyOptions
    {
        [ExcludeFromCodeCoverage]
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }

        [ExcludeFromCodeCoverage]
        public RetryPolicyOptions HttpRetry { get; set; }
    }
}
