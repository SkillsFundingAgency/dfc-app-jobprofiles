using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DFC.App.JobProfile.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsDraftRequest(this HttpRequest request)
        {
            return request != null && request.Path.Value.StartsWith("/draft", StringComparison.OrdinalIgnoreCase);
        }

        public static Uri RequestBaseAddress(this HttpRequest request, IUrlHelper urlHelper)
        {
            if (request != null && urlHelper != null)
            {
                return new Uri($"{request.Scheme}://{request.Host}{urlHelper.Content("~")}");
            }

            return null;
        }
    }
}