using API.Authentication;
using API.Endpoints.Users.RegisterJobSeeker;
using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Users.Register.CompanyEmployee;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Users.Register.RegisterCompanyEmployee;

internal sealed class RegisterCompanyEmployee : IEndpoint
{
    internal sealed record RegisterCompanyEmployeeRequest(
       string FirstName,
       string LastName,
       string CompanyEmail,
       string PhoneNumber,
       string PhoneNumberRegionCode,
       string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/employers/employee/register", async (
            RegisterUserRequest request,
            ICommandHandler<RegisterCompanyEmployeeCommand> handler,
            IUserContext userContext,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterCompanyEmployeeCommand(
                userContext.UserId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.PhoneNumberRegionCode,
                request.Password,
                UserRole.CompanyEmployee);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : result.GetProblem();
        })
        .WithTags("Users")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.CompanyAdmin.ToString()));
    }
}