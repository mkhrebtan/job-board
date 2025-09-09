namespace Application.Abstraction.Storage;

public interface IStorage
{
    Task<string> GetFileUploadUrlAsync(string fileName, string contentType, CancellationToken cancellationToken = default);
}
