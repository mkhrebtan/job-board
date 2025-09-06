using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Contexts.RecruitmentContext.ValueObjects;
using Domain.Repos.Companies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Companies.UpdateLogo;

internal sealed class UpdateCompanyWebsiteCommandHandler : ICommandHandler<UpdateCompanyLogoCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyWebsiteCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCompanyLogoCommand command, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetByIdAsync(new CompanyId(command.Id), cancellationToken);
        if (company is null)
        {
            return Result.Failure(Error.NotFound("Company.NotFound", "The company was not found."));
        }

        if (!Helpers.TryCreateVO(() => LogoUrl.Create(command.LogoUrl), out LogoUrl logoUrl, out Error error))
        {
            return Result.Failure(error);
        }

        var updateResult = company.UpdateLogoUrl(logoUrl);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _companyRepository.Update(company);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
