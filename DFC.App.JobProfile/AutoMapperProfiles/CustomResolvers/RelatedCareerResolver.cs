using AutoMapper;
using DFC.App.JobProfile.Helpers;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Collections.Generic;
using System.Linq;
using DFC.App.JobProfile.Data.Models.Segment.RelatedCareers;

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
            var segmentData = new List<RelatedCareerDataModel>();

            if (source.JobProfileRelatedCareers.IsAny())
            {
                var responseData = source.JobProfileRelatedCareers.FirstOrDefault();

                if (responseData.RelatedCareerProfiles.ContentItems.IsAny())
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
            }

            return segmentData;
        }
    }
}
