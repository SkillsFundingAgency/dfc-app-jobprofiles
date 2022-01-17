using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DFC.App.JobProfile.Extensions
{
    public static class HttpRequestExtensions
    {
        public static Uri GetBaseAddress(this HttpRequest request, IUrlHelper urlHelper = null)
        {
            const string xForwardProto = "x-forwarded-proto";
            const string xOriginalHost = "x-original-host";

            if (request != null)
            {
                request.Headers.TryGetValue(xForwardProto, out var xForwardProtoValue);

                if (string.IsNullOrWhiteSpace(xForwardProtoValue))
                {
                    xForwardProtoValue = request.Scheme ?? Uri.UriSchemeHttp;
                }

                request.Headers.TryGetValue(xOriginalHost, out var xOriginalHostValue);

                if (string.IsNullOrWhiteSpace(xOriginalHostValue))
                {
                    xOriginalHostValue = request.Host.Value;
                }

                return new Uri($"{xForwardProtoValue}://{xOriginalHostValue}{urlHelper?.Content("~")}");
            }

            return null;
        }
    }
}