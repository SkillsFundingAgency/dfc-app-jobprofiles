using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.AutoMapperProfiles.ValueConverters
{
    [ExcludeFromCodeCoverage]
    public class SalaryToStringFormatter : IValueConverter<decimal?, string>
    {
        public string Convert(decimal? sourceMember, ResolutionContext context)
        {
            return sourceMember.HasValue && sourceMember.Value != 0 ? $"{sourceMember}" : "variable";
        }
    }
}
