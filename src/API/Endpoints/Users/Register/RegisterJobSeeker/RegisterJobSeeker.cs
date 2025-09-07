using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Users.Register.JobSeeker;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Users.RegisterJobSeeker;

internal sealed partial class RegisterJobSeeker : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/seeker/register", async (
            RegisterUserRequest request,
            ICommandHandler<RegisterJobSeekerCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterJobSeekerCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.PhoneNumberRegionCode,
                request.Password,
                UserRole.JobSeeker);

            var result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Users");
    }
}
