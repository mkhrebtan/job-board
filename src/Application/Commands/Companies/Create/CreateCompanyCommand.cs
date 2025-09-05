using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.ValueObjects;
using Domain.Repos.Companies;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Companies.Create;

public record CreateCompanyCommand(
    string Name,
    string DescriptionMarkdown,
    string? WebsiteUrl,
    string? LogoUrl,
    int? Size) : ICommand<CreateCompanyCommandResponse>;

public record CreateCompanyCommandResponse(Guid Id);

internal sealed class CreateCompanyCommandHandler : ICommandHandler<CreateCompanyCommand, CreateCompanyCommandResponse>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMarkdownParser _markdownParser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCommandHandler(ICompanyRepository companyRepository, IMarkdownParser markdownParser, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _markdownParser = markdownParser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCompanyCommandResponse>> Handle(CreateCompanyCommand command, CancellationToken cancellationToken = default)
    {
        if (!Helpers.TryCreateVO(() => RichTextContent.Create(command.DescriptionMarkdown, _markdownParser), out RichTextContent description, out Error error))
        {
            return Result<CreateCompanyCommandResponse>.Failure(error);
        }

        WebsiteUrl websiteUrl = default!;
        if (command.WebsiteUrl is null)
        {
            websiteUrl = WebsiteUrl.None;
        }
        else
        {
            if (!Helpers.TryCreateVO(() => WebsiteUrl.Create(command.WebsiteUrl), out websiteUrl, out error))
            {
                return Result<CreateCompanyCommandResponse>.Failure(error);
            }
        }

        LogoUrl logoUrl = default!;
        if (command.LogoUrl is null)
        {
            logoUrl = LogoUrl.None;
        }
        else
        {
            if (!Helpers.TryCreateVO(() => LogoUrl.Create(command.LogoUrl), out logoUrl, out error))
            {
                return Result<CreateCompanyCommandResponse>.Failure(error);
            }
        }

        var companyResult = Company.Create(command.Name, description, websiteUrl, logoUrl, command.Size);
        if (companyResult.IsFailure)
        {
            return Result<CreateCompanyCommandResponse>.Failure(companyResult.Error);
        }

        _companyRepository.Add(companyResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CreateCompanyCommandResponse>.Success(new CreateCompanyCommandResponse(companyResult.Value.Id.Value));
    }
}