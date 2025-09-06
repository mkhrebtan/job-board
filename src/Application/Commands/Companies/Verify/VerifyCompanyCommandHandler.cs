using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Repos.Companies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Companies.Verify;

internal sealed class VerifyCompanyCommandHandler : ICommandHandler<VerifyCompanyCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(VerifyCompanyCommand command, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetByIdAsync(new CompanyId(command.Id), cancellationToken);
        if (company is null)
        {
            return Result.Failure(Error.NotFound("Company.NotFound", "The company was not found."));
        }

        var verifyResult = company.Verify();
        if (verifyResult.IsFailure)
        {
            return Result.Failure(verifyResult.Error);
        }

        _companyRepository.Update(company);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
