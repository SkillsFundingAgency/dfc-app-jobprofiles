﻿using AutoMapper;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
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
            var responseData = source.JobProfileHowToBecome.FirstOrDefault();
            var additionalInfo = new List<AdditionalInformation>();

            switch (routeName)
            {
                case RouteName.Apprenticeship:
                    if (responseData.RelatedApprenticeshipLinks.ContentItems != null &&
                        responseData.RelatedApprenticeshipRequirements.ContentItems != null)
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
                    if (responseData.RelatedCollegeLinks.ContentItems != null &&
                        responseData.RelatedCollegeRequirements.ContentItems != null)
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
                    if (responseData.RelatedUniversityLinks.ContentItems != null &&
                        responseData.RelatedApprenticeshipRequirements.ContentItems != null)
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

            return additionalInfo;
        }
    }
}
