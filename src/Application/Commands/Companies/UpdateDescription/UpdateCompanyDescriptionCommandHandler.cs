using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Repos.Companies;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Companies.UpdateDescription;

internal sealed class UpdateCompanyDescriptionCommandHandler : ICommandHandler<UpdateCompanyDescriptionCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMarkdownParser _markdownParser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyDescriptionCommandHandler(ICompanyRepository companyRepository, IMarkdownParser markdownParser, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _markdownParser = markdownParser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCompanyDescriptionCommand command, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetByIdAsync(command.Id, cancellationToken);
        if (company is null)
        {
            return Result.Failure(Error.NotFound("Company.NotFound", "The company was not found."));
        }

        if (!Helpers.TryCreateVO(() => RichTextContent.Create(command.DescriptionMarkdown, _markdownParser), out RichTextContent description, out Error error))
        {
            return Result.Failure(error);
        }

        var updateResult = company.UpdateDescription(description);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _companyRepository.Update(company);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}