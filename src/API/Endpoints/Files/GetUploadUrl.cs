using Application.Abstraction.Storage;

namespace API.Endpoints.Files;

internal sealed class GetUploadUrl : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("files/upload-url", async (
            string fileName,
            string contentType,
            IStorage storage,
            CancellationToken cancellationToken) =>
        {
            var url = await storage.GetFileUploadUrlAsync(fileName, contentType, cancellationToken);
            return Results.Ok(url);
        })
        .WithTags("Files")
        .RequireAuthorization();
    }
}
