using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Repos.Companies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Companies.UpdateName;

internal sealed class UpdateCompanyNameCommandHandler : ICommandHandler<UpdateCompanyNameCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyNameCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCompanyNameCommand command, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetByIdAsync(new CompanyId(command.Id), cancellationToken);
        if (company is null)
        {
            return Result.Failure(Error.NotFound("Company.NotFound", "The company was not found."));
        }

        var updateResult = company.UpdateName(command.Name);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _companyRepository.Update(company);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}