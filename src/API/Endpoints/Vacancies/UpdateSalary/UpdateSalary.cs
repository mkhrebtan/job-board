using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.UpdateSalary;

namespace API.Endpoints.Vacancies.UpdateSalary;

internal sealed class UpdateSalary : IEndpoint
{
    public record UpdateVacancySalaryRequest(
        decimal MinSalary,
        decimal? MaxSalary,
        string SalaryCurrency);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("vacancies/{vacancyId:guid}/salary", async (
            Guid vacancyId,
            UpdateVacancySalaryRequest request,
            ICommandHandler<UpdateVacancySalaryCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateVacancySalaryCommand(
                vacancyId,
                request.MinSalary,
                request.MaxSalary,
                request.SalaryCurrency);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}
