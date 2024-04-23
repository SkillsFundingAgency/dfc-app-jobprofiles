﻿using AutoMapper;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class FurtherRouteInfoResolver : IValueResolver<JobProfileHowToBecomeResponse, CommonRoutes, string>
    {
        public string Resolve(
            JobProfileHowToBecomeResponse source,
            CommonRoutes destination,
            string destMember,
            ResolutionContext context)
        {
            RouteName routeName = (RouteName)context.Items["RouteName"];
            string furtherRouteInfo = null;

            if (source.JobProfileHowToBecome != null)
            {
                var responseData = source.JobProfileHowToBecome.FirstOrDefault();

                switch (routeName)
                {
                    case RouteName.Apprenticeship:
                        furtherRouteInfo = responseData.ApprenticeshipFurtherRoutesInfo.Html;
                        break;
                    case RouteName.College:
                        furtherRouteInfo = responseData.CollegeFurtherRouteInfo.Html;
                        break;
                    case RouteName.University:
                        furtherRouteInfo = responseData.UniversityFurtherRouteInfo.Html;
                        break;
                }
            }

            return furtherRouteInfo;
        }
    }
}
