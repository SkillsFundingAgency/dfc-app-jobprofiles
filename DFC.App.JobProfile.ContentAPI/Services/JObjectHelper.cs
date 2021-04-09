using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.ContentAPI.Services
{
    [ExcludeFromCodeCoverage]
    internal static class JObjectHelper
    {
        public static TResult GetValue<TResult>(this JObject source, string propertyName)
            where TResult : class =>
                source != null
                    ? source.SelectToken(propertyName)?.ToObject<TResult>()
                    : null;
    }
}
