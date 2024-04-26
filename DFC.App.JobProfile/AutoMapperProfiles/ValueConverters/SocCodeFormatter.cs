using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.AutoMapperProfiles.ValueConverters
{
    [ExcludeFromCodeCoverage]
    public class SocCodeFormatter : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context)
        {
            const int returnLength = 4;

            if (string.IsNullOrWhiteSpace(sourceMember))
            {
                return null;
            }

            if (sourceMember.Length <= returnLength)
            {
                return sourceMember;
            }

            return sourceMember.Substring(0, returnLength);
        }
    }
}
