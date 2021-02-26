using Newtonsoft.Json.Linq;

namespace DFC.App.JobProfile.ContentAPI.Services
{
    internal static class JObjectHelper
    {
        public static TResult GetValue<TResult>(this JObject source, string propertyName)
            where TResult : class =>
                source != null
                    ? source.SelectToken(propertyName)?.ToObject<TResult>()
                    : null;
    }
}
