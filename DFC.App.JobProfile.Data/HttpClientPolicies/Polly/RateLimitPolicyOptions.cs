using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.HttpClientPolicies.Polly
{
    [ExcludeFromCodeCoverage] //This model is only used by startup in polly setup extention methods and hence can not be used in tests.

    public class RateLimitPolicyOptions
    {
        public int Count { get; set; } = 2;

        public int BackoffPower { get; set; } = 45;  //Tested on local machine lower then this is will still get rate limits.
    }
}
