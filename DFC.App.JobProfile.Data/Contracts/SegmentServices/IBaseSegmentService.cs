using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts.SegmentServices
{
    public interface IBaseSegmentService<TModel>
        where TModel : ISegmentDataModel
    {
        Guid DocumentId { get; set; }

        SegmentClientOptions SegmentClientOptions { get; set; }

        Task<HealthCheckItems> HealthCheckAsync();

        Task<string> GetJsonAsync(Guid jobProfileId);

        Task<string> GetMarkupAsync(Guid jobProfileId);
    }
}