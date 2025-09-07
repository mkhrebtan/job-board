using Application.Abstractions.Messaging;
using Application.Commands.Users.Register.CompanyEmployee;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Repos.Companies;
using Domain.Repos.CompanyUsers;
using Domain.Repos.Users;
using Domain.Services;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Users.Register.Employer;

internal sealed class RegisterCompanyEmployeeCommandHandler : ICommandHandler<RegisterCompanyEmployeeCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserService _userService;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyUserRepository _companyUserRepository;
    private readonly CompanyUserService _companyUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCompanyEmployeeCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ICompanyRepository companyRepository,
        ICompanyUserRepository companyUserRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userService = new UserService(_userRepository);
        _companyRepository = companyRepository;
        _companyUserRepository = companyUserRepository;
        _companyUserService = new CompanyUserService(companyUserRepository);
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RegisterCompanyEmployeeCommand command, CancellationToken cancellationToken = default)
    {
        var companyId = await _companyUserRepository.GetCompanyIdByUserId(new UserId(command.CompanyAdminId), cancellationToken);
        if (companyId is null)
        {
            return Result.Failure(Error.NotFound("RegisterCompanyEmployeeCommandHandler.CompanyNotFound", "The company associated with the provided admin ID was not found."));
        }

        var company = await _companyRepository.GetByIdAsync(companyId, cancellationToken);
        if (company is null)
        {
            return Result.Failure(Error.NotFound("RegisterCompanyEmployeeCommandHandler.CompanyNotFound", "The company associated with the provided admin ID was not found."));
        }

        if (!Helpers.TryCreateVO(() => Email.Create(command.Email), out Email email, out Error error))
        {
            return Result.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => PhoneNumber.Create(command.PhoneNumber, command.PhoneNumberRegionCode), out PhoneNumber phoneNumber, out error))
        {
            return Result.Failure(error);
        }

        var userResult = await _userService.CreateUserAsync(
            command.FirstName,
            command.LastName,
            email: email,
            phoneNumber: phoneNumber,
            password: command.Password,
            role: command.UserRole,
            passwordHasher: _passwordHasher,
            ct: cancellationToken);
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error);
        }

        var companyUserResult = await _companyUserService.AssignEmployerToCompanyAsync(userResult.Value, company, cancellationToken);
        if (companyUserResult.IsFailure)
        {
            return Result.Failure(companyUserResult.Error);
        }

        _userRepository.Add(userResult.Value);
        _companyUserRepository.Add(companyUserResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
