using Refit;

namespace Domain.Repositories;

public interface IMediaRepository
{
    Task<(byte[] Content, string MimeType)> GetCountryFlagContent(string countryShortName);

    [Get("/countryflags/{countryShortName}.png")]
    Task<byte[]> GetCountryFlagContentWithRefit(string countryShortName);
}
