using Application.Abstractions.Messaging;
using Application.Commands.Users.Register.Employer;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.ValueObjects;
using Domain.Repos.Companies;
using Domain.Repos.CompanyUsers;
using Domain.Repos.Users;
using Domain.Services;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Users.Register.CompanyEmployee;

internal sealed class RegisterCompanyAdminCommandHandler : ICommandHandler<RegisterCompanyAdminCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserService _userService;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyUserRepository _companyUserRepository;
    private readonly CompanyUserService _companyUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCompanyAdminCommandHandler(
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

    public async Task<Result> Handle(RegisterCompanyAdminCommand command, CancellationToken cancellationToken = default)
    {
        if (!Helpers.TryCreateVO(() => Email.Create(command.CompanyEmail), out Email email, out Error error))
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

        var companyResult = Company.Create(command.CompanyName, RichTextContent.Empty, WebsiteUrl.None, LogoUrl.None, default);
        if (companyResult.IsFailure)
        {
            return Result.Failure(companyResult.Error);
        }

        var companyUserResult = await _companyUserService.AssignEmployerToCompanyAsync(userResult.Value, companyResult.Value, cancellationToken);
        if (companyUserResult.IsFailure)
        {
            return Result.Failure(companyUserResult.Error);
        }

        _userRepository.Add(userResult.Value);
        _companyRepository.Add(companyResult.Value);
        _companyUserRepository.Add(companyUserResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}