using DFC.App.Services.Common.Factories;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Providers;
using DFC.App.Services.Common.Registration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Adapters
{
    public sealed class ActionResultOperationsAdapter :
        IAdaptActionResultOperations,
        IRequireServiceRegistration
    {
        public ActionResultOperationsAdapter(
            IProvideSafeOperations safeOperations,
            IProvideFaultResponses responseProvider,
            ICreateLoggingContexts loggingFactory)
        {
            It.IsNull(safeOperations)
                .AsGuard<ArgumentNullException>(nameof(safeOperations));
            It.IsNull(responseProvider)
                .AsGuard<ArgumentNullException>(nameof(responseProvider));
            It.IsNull(loggingFactory)
                .AsGuard<ArgumentNullException>(nameof(loggingFactory));

            Operation = safeOperations;
            Faulted = responseProvider;
            Logging = loggingFactory;
        }

        internal IProvideSafeOperations Operation { get; }

        internal IProvideFaultResponses Faulted { get; }

        internal ICreateLoggingContexts Logging { get; }

        public async Task<IActionResult> Run(Func<Task<HttpResponseMessage>> coordinatedAction, string traceMessage)
        {
            using (var loggingScope = Logging.BeginLoggingFor(traceMessage))
            {
                return await Operation.Try(coordinatedAction, x => ProcessError(x, loggingScope))
                    .AsActionResult();
            }
        }

        public async Task<IActionResult> Run<TModel>(
            Func<Task<HttpResponseMessage>> coordinatedAction,
            Func<TModel, IActionResult> contentResult,
            string traceMessage)
                where TModel : class
        {
            using (var loggingScope = Logging.BeginLoggingFor(traceMessage))
            {
                return await Operation.Try(coordinatedAction, x => ProcessError(x, loggingScope))
                        .AsActionResult(contentResult);
            }
        }

        internal async Task<HttpResponseMessage> ProcessError(Exception exception, ILoggingContextScope loggingContext) =>
            await Faulted.GetResponseFor(exception, loggingContext);
    }
}
