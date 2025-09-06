using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Contexts.RecruitmentContext.ValueObjects;
using Domain.Repos.Companies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Companies.UpdateWebsite;

internal sealed class UpdateCompanyWebsiteCommandHandler : ICommandHandler<UpdateCompanyWebsiteCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyWebsiteCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCompanyWebsiteCommand command, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetByIdAsync(new CompanyId(command.Id), cancellationToken);
        if (company is null)
        {
            return Result.Failure(Error.NotFound("Company.NotFound", "The company was not found."));
        }

        if (!Helpers.TryCreateVO(() => WebsiteUrl.Create(command.WebsiteUrl), out WebsiteUrl websiteUrl, out Error error))
        {
            return Result.Failure(error);
        }

        var updateResult = company.UpdateWebsiteUrl(websiteUrl);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _companyRepository.Update(company);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
