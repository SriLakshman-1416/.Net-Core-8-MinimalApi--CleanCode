using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories;

public class MediaRepository(IHttpClientFactory httpClientFactory) : IMediaRepository
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<(byte[] Content,string MimeType)>GetCountryFlagContent(string countryShortName)
    {
        byte[] fileBytes;

        using HttpClient client = _httpClientFactory.CreateClient();
        fileBytes = await client.GetByteArrayAsync($"https://anthonygiretti.blob.core.windows.net/countryflags/{countryShortName}.png");

        return (fileBytes, "image/png");
    }

    public Task<byte[]> GetCountryFlagContentWithRefit(string countryShortName)
    {
        throw new NotImplementedException();
    }
}
