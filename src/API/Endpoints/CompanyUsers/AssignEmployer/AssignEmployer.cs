using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.CompanyUsers.AssignEmployer;

namespace API.Endpoints.CompanyUsers.AssignEmployer;

internal sealed class AssignEmployer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/company-users/{userId:guid}/assign-employer/{companyId:guid}", async (
            Guid userId,
            Guid companyId,
            ICommandHandler<AssignEmployerCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AssignEmployerCommand(userId, companyId);
            var result = await handler.Handle(command, cancellationToken);
            if (result.IsFailure)
            {
                return result.GetProblem();
            }

            return Results.NoContent();
        })
        .WithTags("CompanyUsers");
    }
}
