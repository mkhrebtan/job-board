using Amazon.S3;
using Amazon.S3.Model;
using Application.Abstraction.Storage;
using Domain.Shared.ErrorHandling;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage;

internal sealed class S3Storage : IStorage
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3Settings> _s3Settings;

    public S3Storage(IAmazonS3 s3Client, IOptions<S3Settings> s3Settings)
    {
        _s3Client = s3Client;
        _s3Settings = s3Settings;
    }

    public async Task<string> GetFileUploadUrlAsync(string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var key = Guid.NewGuid();
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _s3Settings.Value.BucketName,
            Key = $"files/{key}",
            Verb = HttpVerb.PUT,
            Expires = DateTime.Now.AddHours(1),
            ContentType = contentType,
            Metadata =
            {
                ["file-name"] = fileName,
            },
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }
}
