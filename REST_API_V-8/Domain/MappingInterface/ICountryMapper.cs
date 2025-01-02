using Coding_Clean_Safe_REST_API_V_8_MinimalApi.Models;
using Domain.Dtos;

namespace Domain.MappingInterface;

public interface ICountryMapper
{
    public CountryDto MapModelToDto(Country country);
    public Country MapDtoToModel(CountryDto country);
    public List<Country> MapListOfDtoToModel(List<CountryDto> countries);

}
