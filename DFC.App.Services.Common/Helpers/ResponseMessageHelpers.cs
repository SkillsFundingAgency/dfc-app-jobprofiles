using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Helpers
{
    public static class ResponseMessageHelpers
    {
        public const string ResponseContentType = "application/json";

        public static HttpResponseMessage SetContent(this HttpResponseMessage source, string content)
        {
            source.Content = new StringContent(content, Encoding.UTF8, ResponseContentType);
            return source;
        }

        public static async Task<IActionResult> AsActionResult(this Task<HttpResponseMessage> source)
        {
            var response = await source;
            return new HttpResponseMessageResult(response);
        }

        public static async Task<IActionResult> AsActionResult<TModel>(
            this Task<HttpResponseMessage> source,
            Func<TModel, IActionResult> contentResult)
                where TModel : class
        {
            var response = await source;
            var model = JsonConvert.DeserializeObject<TModel>(await response.Content.ReadAsStringAsync());
            return contentResult(model);
        }
    }
}