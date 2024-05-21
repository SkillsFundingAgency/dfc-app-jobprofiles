﻿using DFC.App.JobProfile.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IJobProfileService
    {
        Task<bool> PingAsync();

        Task<IList<HealthCheckItem>> SegmentsHealthCheckAsync();

        Task<IEnumerable<Models.JobProfileModel>> GetAllAsync();

        Task<Models.JobProfileModel> GetByIdAsync(Guid documentId);

        Task<Models.JobProfileModel> GetByNameAsync(string canonicalName);

        Task<Models.JobProfileModel> GetByAlternativeNameAsync(string alternativeName);

        Task<HttpStatusCode> Create(Models.JobProfileModel jobProfileModel);

        Task<HttpStatusCode> Update(Models.JobProfileModel jobProfileModel);

        Task<HttpStatusCode> Update(Models.JobProfileMetadata jobProfileMetadata);

        Task<HttpStatusCode> RefreshSegmentsAsync(RefreshJobProfileSegment refreshJobProfileSegmentModel);

        Task<bool> DeleteAsync(Guid documentId);

        Task<SegmentModel> GetHowToBecomeSegmentAsync(string canonicalName, string filter);

        Task<SegmentModel> GetOverviewSegment(string canonicalName, string filter);

        Task<bool> RefreshCourses(string filter, int first, int skip);

        Task<bool> RefreshApprenticeshipsAsync(string filter, int first, int skip);

        Task<bool> RefreshAllSegments(string filter, int first, int skip);

        Task<int> CountJobProfiles(string filter);
    }
}