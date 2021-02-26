using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.DocumentStore
{
    public interface IManageDocumentStorage<TModel>
        where TModel : IContentPageModel
    {
        Task<bool> Ping();

        Task<TModel> Get(Expression<Func<TModel, bool>> where);

        Task<IReadOnlyCollection<TModel>> GetAll();

        Task<HttpStatusCode> Upsert(TModel model);

        Task<HttpStatusCode> Delete(Guid documentID);
    }
}
