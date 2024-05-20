using AutoMapper;
using DFC.App.JobProfile.Data.Models.Segment.CurrentOpportunities;

namespace DFC.App.JobProfile.AutoMapperProfiles.CustomResolvers
{
    public class VacancyLocationResolver : IValueResolver<ApprenticeshipVacancySummary, Vacancy, Location>
    {
        public Location Resolve(
            ApprenticeshipVacancySummary source,
            Vacancy destination,
            Location destMember,
            ResolutionContext context)
        {
            Location location = null;
            string town = null;

            if (source != null && source.Address != null)
            {
                if (source.Address.AddressLine2 != null && source.Address.AddressLine3 != null && source.Address.AddressLine4 != null)
                {
                    town = source.Address.AddressLine2 + ", " + source.Address.AddressLine3 + ", " + source.Address.AddressLine4;
                }
                else if (source.Address.AddressLine2 != null && source.Address.AddressLine3 != null)
                {
                    town = source.Address.AddressLine2 + ", " + source.Address.AddressLine3;
                }
                else if (source.Address.AddressLine2 != null)
                {
                    town = source.Address.AddressLine2;
                }
                else if (source.Address.AddressLine3 != null)
                {
                    town = source.Address.AddressLine3;
                }

                location = new Location
                {
                    PostCode = source.Address.PostCode,
                    Town = town,
                };
            }

            return location;
        }
    }
}
