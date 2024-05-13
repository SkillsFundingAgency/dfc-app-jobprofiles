using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.HttpClientPolicies
{
    [ExcludeFromCodeCoverage] //This model is only used by startup in polly setup extention methods and hence can not be used in tests.
    public class RetryPolicyOptions
    {
        public int Count { get; set; } = 3;

        public int BackoffPower { get; set; } = 2;
    }
}
