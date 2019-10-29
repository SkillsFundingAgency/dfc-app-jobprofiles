using DFC.App.JobProfile.Data;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.MessageFunctionApp.Services
{
    public interface IHttpClientService<TModel>
        where TModel : class, new()
    {
        Task<TModel> GetByIdAsync(Guid id);

        Task<HttpStatusCode> PostAsync<TInput>(TInput postModel, string postEndpoint = "profile")
            where TInput : BaseJobProfile;

        Task<HttpStatusCode> DeleteAsync(Guid id);

        Task<HttpStatusCode> PatchAsync<TInput>(TInput patchModel, string patchTypeEndpoint = "metadata")
            where TInput : BaseJobProfile;
    }
}