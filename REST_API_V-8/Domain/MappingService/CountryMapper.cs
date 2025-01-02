using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Models;
using Domain.Dtos;
using Domain.MappingInterface;

namespace Domain.MappingService
{
    public class CountryMapper : ICountryMapper
    {
        public CountryDto? MapModelToDto(Country country)
        {
            return country is not null ? new CountryDto
            {
                Name = country.Name,
                Description = country.Description,
                FlagUri = country.FlagUri,
            } : null;
        }
        public Country? MapDtoToModel(CountryDto country)
        {
            return country != null ? new Country
            {
                Id = country.Id,
                Name = country.Name,
                Description = country.Description,
                FlagUri = country.FlagUri,
            } : null;
        }

        public List<Country> MapListOfDtoToModel(List<CountryDto> countries)
        {
            return countries.Select(MapDtoToModel).ToList();
        }
    }
}
