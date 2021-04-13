using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Adapters
{
    public interface IAdaptActionResultOperations
    {
        Task<IActionResult> Run(Func<Task<HttpResponseMessage>> coordinatedAction, string traceMessage);

        Task<IActionResult> Run<TModel>(
            Func<Task<HttpResponseMessage>> coordinatedAction,
            Func<TModel, IActionResult> contentResult,
            string traceMessage)
                where TModel : class;
    }
}
