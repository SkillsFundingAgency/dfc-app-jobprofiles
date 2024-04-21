using AutoMapper;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models.HowToBecome;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class FurtherRouteInfoResolver : IValueResolver<JobProfileHowToBecomeResponse, CommonRoutes, string>
    {
        public string Resolve(JobProfileHowToBecomeResponse source, CommonRoutes destination, string destMember, ResolutionContext context)
        {
            RouteName routeName = (RouteName)context.Items["RouteName"];
            var furtherRouteInfo = string.Empty;

            switch (routeName)
            {
                case RouteName.Apprenticeship:
                    furtherRouteInfo = source.JobProfileHowToBecome.FirstOrDefault()?.Apprenticeshipfurtherroutesinfo.Html;
                    break;
                case RouteName.College:
                    furtherRouteInfo = source.JobProfileHowToBecome.FirstOrDefault()?.Collegefurtherrouteinfo.Html;
                    break;
                case RouteName.University:
                    furtherRouteInfo = source.JobProfileHowToBecome.FirstOrDefault()?.Universityfurtherrouteinfo.Html;
                    break;
            }

            return furtherRouteInfo;
        }
    }
}
