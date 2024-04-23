﻿using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class RealStoryResolver : IValueResolver<JobProfileHowToBecomeResponse, HowToBecomeSegmentDataModel, RealStory>
    {
        public RealStory Resolve(
            JobProfileHowToBecomeResponse source,
            HowToBecomeSegmentDataModel destination,
            RealStory destMember,
            ResolutionContext context)
        {
            RealStory realStory = null;

            if (source.JobProfileHowToBecome != null)
            {
                var responseData = source.JobProfileHowToBecome.FirstOrDefault();

                if (responseData.RealStory.ContentItems.Any())
                {
                    var realStoryData = responseData.RealStory.ContentItems.FirstOrDefault();

                    realStory = new RealStory
                    {
                        Title = realStoryData.DisplayText,
                        Summary = realStoryData.Text,
                        BodyHtml = realStoryData.Body.Html,
                        FurtherInformationHtml = realStoryData.FurtherInformation.Html,
                        Thumbnail = new Thumbnail
                        {
                            Text = realStoryData.Thumbnail.Paths,
                            Url = realStoryData.Thumbnail.Urls,
                        },
                    };
                }
            }

            return realStory;
        }
    }
}
