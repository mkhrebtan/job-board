using API.Authentication;
using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Repos.Companies;
using Domain.Repos.CompanyUsers;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Companies.UpdateSize;

internal sealed class UpdateCompanySizeCommandHandler : ICommandHandler<UpdateCompanySizeCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ICompanyUserRepository _companyUserRepository;

    public UpdateCompanySizeCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork, IUserContext userContext, ICompanyUserRepository companyUserRepository)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _companyUserRepository = companyUserRepository;
    }

    public async Task<Result> Handle(UpdateCompanySizeCommand command, CancellationToken cancellationToken = default)
    {
        var companyId = await _companyUserRepository.GetCompanyIdByUserId(new UserId(_userContext.UserId), cancellationToken);
        if (companyId is null || companyId.Value != command.Id)
        {
            return Result.Failure(Error.Problem("Company.Forbidden", "You do not have permission to update this company."));
        }

        var company = await _companyRepository.GetByIdAsync(new CompanyId(command.Id), cancellationToken);
        if (company is null)
        {
            return Result.Failure(Error.NotFound("Company.NotFound", "The company was not found."));
        }

        var updateResult = company.UpdateSize(command.Size);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _companyRepository.Update(company);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}