using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ICmsApiService
    {
        Task<IList<T>> GetSummaryAsync<T>()
            where T : class;

        Task<T> GetItemAsync<T>(Uri url)
            where T : class, IPagesApiDataModel;

        Task<ApiContentItemModel> GetContentItemAsync(LinkDetails details);

        Task<ApiContentItemModel> GetContentItemAsync(Uri uri);

        Task<List<T>> GetContentAsync<T>()
            where T : class;
    }
}
