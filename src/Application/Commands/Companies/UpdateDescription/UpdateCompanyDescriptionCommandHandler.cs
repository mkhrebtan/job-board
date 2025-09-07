using API.Authentication;
using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Repos.Companies;
using Domain.Repos.CompanyUsers;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Companies.UpdateDescription;

internal sealed class UpdateCompanyDescriptionCommandHandler : ICommandHandler<UpdateCompanyDescriptionCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMarkdownParser _markdownParser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ICompanyUserRepository _companyUserRepository;

    public UpdateCompanyDescriptionCommandHandler(
        ICompanyRepository companyRepository,
        IMarkdownParser markdownParser,
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        ICompanyUserRepository companyUserRepository)
    {
        _companyRepository = companyRepository;
        _markdownParser = markdownParser;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _companyUserRepository = companyUserRepository;
    }

    public async Task<Result> Handle(UpdateCompanyDescriptionCommand command, CancellationToken cancellationToken = default)
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