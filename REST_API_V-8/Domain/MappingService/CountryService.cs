using Domain.Dtos;
using Domain.MappingInterface;

namespace Domain.MappingService;

public class CountryService : ICountryService
{
    public int CreateOrUpdate(CountryDto country)
    {
        throw new NotImplementedException();
    }

    public bool Delete(int id)
    {
        throw new NotImplementedException();
    }

    public List<CountryDto> GetAll()
    {
        throw new NotImplementedException();
    }

    public (byte[] fileContent, string mimeType, string filename) GetFile()
    {
        throw new NotImplementedException();
    }

    public CountryDto Retrieve(int id)
    {
        throw new NotImplementedException();
    }

    public bool UpdateDescription(int id, string description)
    {
        throw new NotImplementedException();
    }
}
