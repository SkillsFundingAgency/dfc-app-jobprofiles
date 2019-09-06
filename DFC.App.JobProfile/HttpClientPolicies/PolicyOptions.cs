namespace DFC.App.JobProfile.HttpClientPolicies
{
    public class PolicyOptions
    {
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }

        public RetryPolicyOptions HttpRetry { get; set; }
    }
}
