using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IBaseSegmentService<TModel>
    {
        Guid DocumentId { get; set; }

        SegmentClientOptions SegmentClientOptions { get; set; }

        Task<TModel> LoadDataAsync();

        Task<string> LoadMarkupAsync();

        Task<HealthCheckItems> HealthCheckAsync();
    }
}