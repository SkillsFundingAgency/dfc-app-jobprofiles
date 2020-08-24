using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ICmsApiService
    {
        Task<IList<PagesSummaryItemModel>> GetSummaryAsync();

        Task<PagesApiDataModel> GetItemAsync(Uri url);

        Task<PagesApiContentItemModel> GetContentItemAsync(LinkDetails details);

        Task<PagesApiContentItemModel> GetContentItemAsync(Uri uri);

        Task<List<StaticContentItemModel>> GetContentAsync();
    }
}
