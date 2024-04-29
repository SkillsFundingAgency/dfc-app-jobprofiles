using AutoMapper;
using DFC.App.JobProfile.Data.Models.Segment.Tasks;
using DFC.App.JobProfile.Helpers;
using DFC.App.JobProfile.ProfileService.Utilities;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class UniformResolver : FormatContentService, IValueResolver<JobProfileWhatYoullDoResponse, TasksSegmentDataModel, string>
    {
        public string Resolve(
            JobProfileWhatYoullDoResponse source,
            TasksSegmentDataModel destination,
            string destMember,
            ResolutionContext context)
        {
            var wordList = new List<string>();

            if (source.JobProfileWhatYoullDo.IsAny())
            {
                var responseData = source.JobProfileWhatYoullDo.FirstOrDefault();

                if (responseData.RelatedUniforms.ContentItems.IsAny())
                {
                    foreach (var contentItem in responseData.RelatedUniforms.ContentItems)
                    {
                        wordList.Add(contentItem.Description);
                    }

                    return Convert(wordList);
                }
            }

            return string.Empty;
        }

        public string Convert(IEnumerable<string> wordList)
        {
            const string UniformLeadingText = "You may need to wear";
            const string UniformConjunction = "and";

            return GetParagraphText(UniformLeadingText, wordList, UniformConjunction);
        }
    }
}
