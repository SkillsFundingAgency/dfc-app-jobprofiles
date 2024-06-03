using AutoMapper;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.App.JobProfile.Helpers;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class AdditionalInfoResolver : IValueResolver<JobProfileHowToBecomeResponse, CommonRoutes, List<AdditionalInformation>>
    {
        public List<AdditionalInformation> Resolve(
            JobProfileHowToBecomeResponse source,
            CommonRoutes destination,
            List<AdditionalInformation> destMember,
            ResolutionContext context)
        {
            RouteName routeName = (RouteName)context.Items["RouteName"];
            var additionalInfo = new List<AdditionalInformation>();

            if (source != null && source.JobProfileHowToBecome.IsAny())
            {
                var responseData = source.JobProfileHowToBecome.FirstOrDefault();

                switch (routeName)
                {
                    case RouteName.Apprenticeship:
                        if (responseData.RelatedApprenticeshipLinks.ContentItems.IsAny() &&
                            responseData.RelatedApprenticeshipRequirements.ContentItems.IsAny())
                        {
                            foreach (var item in responseData.RelatedApprenticeshipLinks.ContentItems)
                            {
                                additionalInfo.Add(new AdditionalInformation
                                {
                                    Link = item.URL,
                                    Text = item.Text,
                                });
                            }
                        }

                        break;
                    case RouteName.College:
                        if (responseData.RelatedCollegeLinks.ContentItems.IsAny() &&
                            responseData.RelatedCollegeRequirements.ContentItems.IsAny())
                        {
                            foreach (var item in responseData.RelatedCollegeLinks.ContentItems)
                            {
                                additionalInfo.Add(new AdditionalInformation
                                {
                                    Link = item.URL,
                                    Text = item.Text,
                                });
                            }
                        }

                        break;
                    case RouteName.University:
                        if (responseData.RelatedUniversityLinks.ContentItems.IsAny() &&
                            responseData.RelatedApprenticeshipRequirements.ContentItems.IsAny())
                        {
                            foreach (var item in responseData.RelatedUniversityLinks.ContentItems)
                            {
                                additionalInfo.Add(new AdditionalInformation
                                {
                                    Link = item.URL,
                                    Text = item.Text,
                                });
                            }
                        }

                        break;
                }
            }

            return additionalInfo;
        }
    }
}
