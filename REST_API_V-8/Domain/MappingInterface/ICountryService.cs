using Domain.Dtos;

namespace Domain.MappingInterface;

public interface ICountryService
{
    CountryDto Retrieve(int id);
    List<CountryDto> GetAll();
    int CreateOrUpdate(CountryDto country);
    bool UpdateDescription(int id, string description);
    bool Delete(int id);

    (
        byte[] fileContent,
        string mimeType,
        string filename
    ) GetFile();
}
