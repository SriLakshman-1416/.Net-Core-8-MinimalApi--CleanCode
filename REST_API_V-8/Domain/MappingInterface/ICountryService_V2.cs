using Domain.Dtos;

namespace Domain.MappingInterface;

public interface ICountryService_V2
{
    Task<int> CreateOrUpdateAsync(CountryDto country);
    Task<CountryDto> RetrieveAsync(int id);
    Task<List<CountryDto>> GetAllAsync();
    Task<bool> UpdateDescriptionAsync(int id, string description);
    Task<bool> DeleteAsync(int id);
}
