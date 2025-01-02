using Domain.Dtos;
using Domain.MappingInterface;
using Domain.Repositories;

namespace BusinessLogicLayer.Services;

public class CountryService_V2(ICountryRepository countryRepository) : ICountryService_V2
{
    private readonly ICountryRepository _countryRepository = countryRepository;

    public async Task<bool> DeleteAsync(int id)
    {
        return await _countryRepository.DeleteAsync(id) > 0;
    }

    public async Task<List<CountryDto>> GetAllAsync()
    {
        return await _countryRepository.GetAllAsync();
    }

    public async Task<CountryDto> RetrieveAsync(int id)
    {
        return await _countryRepository.RetrieveAsync(id);
    }

    public async Task<int> CreateOrUpdateAsync(CountryDto country)
    {
        if (country?.Id is null)
            return await _countryRepository.CreateAsync(country);


        if (await _countryRepository.UpdateAsync(country) > 0)
            return country.Id;

        return 0;
    }


    public async Task<bool> UpdateDescriptionAsync(int id, string description)
    {
        return await _countryRepository.UpdateDescriptionAsync(id, description) > 0;
    }
}
