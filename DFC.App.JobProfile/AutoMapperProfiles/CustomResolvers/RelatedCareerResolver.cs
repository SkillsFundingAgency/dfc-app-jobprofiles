using AutoMapper;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models.RelatedCareersModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class RelatedCareerResolver : IValueResolver<RelatedCareersResponse, RelatedCareerSegmentDataModel, List<RelatedCareerDataModel>>
    {
        public List<RelatedCareerDataModel> Resolve(
            RelatedCareersResponse source,
            RelatedCareerSegmentDataModel destination,
            List<RelatedCareerDataModel> destMember,
            ResolutionContext context)
        {
            var responseData = source.JobProfileRelatedCareers.FirstOrDefault();
            var segmentData = new List<RelatedCareerDataModel>();

            if (responseData.RelatedCareerProfiles.ContentItems != null)
            {
                foreach (var career in responseData.RelatedCareerProfiles.ContentItems)
                {
                    segmentData.Add(new RelatedCareerDataModel
                    {
                        ProfileLink = career.PageLocation.FullUrl,
                        Title = career.DisplayText,
                    });
                }
            }
            return segmentData;
        }
    }
}
