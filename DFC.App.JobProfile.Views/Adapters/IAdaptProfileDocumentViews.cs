using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ViewSupport.Adapters
{
    public interface IAdaptProfileDocumentViews
    {
        Task<IActionResult> GetSummaryDocuments<TModel>(
            Func<TModel, IActionResult> contentResult)
                where TModel : class;

        Task<IActionResult> GetDocumentViewFor<TModel>(
            string occupationName,
            Func<TModel, IActionResult> contentResult)
                where TModel : class;
    }
}