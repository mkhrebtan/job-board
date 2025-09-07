using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Users.Register.Employer;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Users.Register.RegisterCompanyAdmin;

internal sealed class RegisterCompanyAdmin : IEndpoint
{
    internal sealed record RegisterCompanyAdminRequest(
       string FirstName,
       string LastName,
       string CompanyEmail,
       string CompanyName,
       string PhoneNumber,
       string PhoneNumberRegionCode,
       string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/employers/admin/register", async (
            RegisterCompanyAdminRequest request,
            ICommandHandler<RegisterCompanyAdminCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterCompanyAdminCommand(
                request.FirstName,
                request.LastName,
                request.CompanyEmail,
                request.CompanyName,
                request.PhoneNumber,
                request.PhoneNumberRegionCode,
                request.Password,
                UserRole.CompanyAdmin);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Users");
    }
}
