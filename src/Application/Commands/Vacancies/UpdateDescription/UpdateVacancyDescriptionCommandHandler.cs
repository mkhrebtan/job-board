using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Repos.Vacancies;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Vacancies.UpdateDescription;

internal sealed class UpdateVacancyDescriptionCommandHandler : ICommandHandler<UpdateVacancyDescriptionCommand>
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMarkdownParser _markdownParser;

    public UpdateVacancyDescriptionCommandHandler(IVacancyRepository vacancyRepository, IUnitOfWork unitOfWork, IMarkdownParser markdownParser)
    {
        _vacancyRepository = vacancyRepository;
        _unitOfWork = unitOfWork;
        _markdownParser = markdownParser;
    }

    public async Task<Result> Handle(UpdateVacancyDescriptionCommand command, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(new VacancyId(command.Id), cancellationToken);
        if (vacancy is null)
        {
            return Result.Failure(Error.NotFound("Vacancy.NotFound", $"Vacancy with ID {command.Id} was not found."));
        }

        var descriptionResult = RichTextContent.Create(command.descriptionMarkdown, _markdownParser);
        if (descriptionResult.IsFailure)
        {
            return Result.Failure(descriptionResult.Error);
        }

        var result = vacancy.UpdateDescripiton(descriptionResult.Value);
        if (result.IsFailure)
        {
            return result;
        }

        _vacancyRepository.Update(vacancy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}