using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.UpdateCategory;

namespace API.Endpoints.Vacancies.UpdateCategory;

internal sealed class UpdateCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("vacancies/{vacancyId:guid}/category/{categoryId:guid}", async (
            Guid vacancyId,
            Guid categoryId,
            ICommandHandler<UpdateVacancyCategoryCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateVacancyCategoryCommand(vacancyId, categoryId);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}
