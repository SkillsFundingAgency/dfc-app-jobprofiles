using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IStaticCosmosRepository<T>
        where T : IDataModel
    {
        Task<bool> PingAsync();

        Task<T> GetAsync(Expression<Func<T, bool>> where);

        Task<IEnumerable<T>> GetAllAsync();

        Task<HttpStatusCode> UpsertAsync(T model);

        Task<HttpStatusCode> DeleteAsync(Guid documentId);
    }
}
