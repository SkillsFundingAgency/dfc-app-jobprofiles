using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DFC.App.JobProfile.Data.Models.Segment.SkillsModels;
using DFC.App.JobProfile.Helpers;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class SkillsResolver : IValueResolver<JobProfileSkillsResponse, JobProfileSkillSegmentDataModel, List<Restriction>>
    {
        public List<Restriction> Resolve(
            JobProfileSkillsResponse source,
            JobProfileSkillSegmentDataModel destination,
            List<Restriction> destMember,
            ResolutionContext context)
        {
            var restrictions = new List<Restriction>();

            if (source.JobProfileSkills.IsAny())
            {
                var responseData = source.JobProfileSkills.SelectMany(d => d.Relatedrestrictions.ContentItems).ToList();

                if (responseData.IsAny())
                {
                    foreach (var restriction in responseData)
                    {
                        restrictions.Add(new Restriction
                        {
                            Description = restriction.Info.Html,
                            Title = restriction.DisplayText,
                            Id = Guid.Parse(restriction.GraphSync.NodeId.Substring(restriction.GraphSync.NodeId.LastIndexOf('/') + 1)),
                        });
                    }
                }
            }

            return restrictions;
        }
    }
}
