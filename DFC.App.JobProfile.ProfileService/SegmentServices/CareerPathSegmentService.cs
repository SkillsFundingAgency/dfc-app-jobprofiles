using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments.CareerPathModels;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService.SegmentServices
{
    public class CareerPathSegmentService : BaseSegmentService<CareerPathSegmentDataModel, CareerPathSegmentService>, ICareerPathSegmentService
    {
        public CareerPathSegmentService(
                                            HttpClient httpClient,
                                            ILogger<CareerPathSegmentService> logger,
                                            CareerPathSegmentClientOptions careerPathSegmentClientOptions)
        : base(httpClient, logger, careerPathSegmentClientOptions)
        {
        }
    }
}
