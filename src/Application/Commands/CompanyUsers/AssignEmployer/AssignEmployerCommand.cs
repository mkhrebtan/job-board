using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Repos.Companies;
using Domain.Repos.CompanyUsers;
using Domain.Repos.Users;
using Domain.Services;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.CompanyUsers.AssignEmployer;

public record AssignEmployerCommand(Guid EmployerId, Guid CompanyId) : ICommand;

internal sealed class AssignEmployerCommandHandler : ICommandHandler<AssignEmployerCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyUserRepository _companyUserRepository;
    private readonly CompanyUserService _companyUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignEmployerCommandHandler(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        ICompanyUserRepository companyUserRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _companyUserRepository = companyUserRepository;
        _companyUserService = new CompanyUserService(companyUserRepository);
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignEmployerCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(command.EmployerId), cancellationToken);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("AssignEmployer.UserNotFound", "The specified user was not found."));
        }

        var company = await _companyRepository.GetByIdAsync(new CompanyId(command.CompanyId), cancellationToken);
        if (company is null)
        {
            return Result.Failure(Error.NotFound("AssignEmployer.CompanyNotFound", "The specified company was not found."));
        }

        var companyUserResult = await _companyUserService.AssignEmployerToCompanyAsync(user, company, cancellationToken);
        if (companyUserResult.IsFailure)
        {
            return Result.Failure(companyUserResult.Error);
        }

        _companyUserRepository.Add(companyUserResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
