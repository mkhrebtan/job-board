using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Repos.Companies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Companies.UpdateSize;

public record UpdateCompanySizeCommand(Guid Id, int Size) : ICommand;

internal sealed class UpdateCompanySizeCommandHandler : ICommandHandler<UpdateCompanySizeCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanySizeCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCompanySizeCommand command, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetByIdAsync(command.Id, cancellationToken);
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