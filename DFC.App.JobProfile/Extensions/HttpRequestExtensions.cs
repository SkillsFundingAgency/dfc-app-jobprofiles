using Microsoft.AspNetCore.Http;
using System;

namespace DFC.App.JobProfile.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsDraftRequest(this HttpRequest request)
        {
            return request != null && request.Path.Value.StartsWith("/draft", StringComparison.OrdinalIgnoreCase);
        }
    }
}