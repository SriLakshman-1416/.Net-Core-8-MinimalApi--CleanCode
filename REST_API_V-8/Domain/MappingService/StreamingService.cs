using Domain.MappingInterface;

namespace Domain.MappingService
{
    public class StreamingService : IStreamingService
    {
        public Task<(Stream stream, string mimeType)> GetFileStream()
        {
            throw new NotImplementedException();
        }
    }
}
