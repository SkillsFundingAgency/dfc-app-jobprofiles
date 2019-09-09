using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models.Segments;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DFC.App.JobProfile.ProfileService
{
    public class CareerPathSegmentService : BaseSegmentService<CareerPathSegmentModel, CareerPathSegmentService>, ICareerPathSegmentService
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
