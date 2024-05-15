using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.HttpClientPolicies.Polly
{
    [ExcludeFromCodeCoverage] //This model is only used by startup in polly setup extention methods and hence can not be used in tests.
    public class CorePolicyOptions
    {
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }

        public RetryPolicyOptions HttpRetry { get; set; }

        public RateLimitPolicyOptions HttpRateLimitRetry { get; set; }
    }
}
