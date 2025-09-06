using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.UpdateLocation;

namespace API.Endpoints.Vacancies.UpdateLocation;

internal sealed class UpdateLocation : IEndpoint
{
    public record UpdateVacancyLocationRequest(
        string Country,
        string City,
        string? Region,
        string? District,
        string? Address,
        decimal? Latitude,
        decimal? Longitude);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("vacancies/{vacancyId:guid}/location", async (
            Guid vacancyId,
            UpdateVacancyLocationRequest request,
            ICommandHandler<UpdateVacancyLocationCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateVacancyLocationCommand(
                vacancyId,
                request.Country,
                request.City,
                request.Region,
                request.District,
                request.Address,
                request.Latitude,
                request.Longitude);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}
