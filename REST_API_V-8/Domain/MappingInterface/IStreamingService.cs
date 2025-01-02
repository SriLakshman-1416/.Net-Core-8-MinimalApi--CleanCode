namespace Domain.MappingInterface;

public interface IStreamingService
{
    Task<(Stream stream, string mimeType)> GetFileStream();
}
