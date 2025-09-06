using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Repos.Vacancies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Vacancies.Publish;

internal class PublishVacancyCommandHandler : ICommandHandler<PublishVacancyCommand>
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishVacancyCommandHandler(IVacancyRepository vacancyRepository, IUnitOfWork unitOfWork)
    {
        _vacancyRepository = vacancyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(PublishVacancyCommand command, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(new VacancyId(command.Id), cancellationToken);
        if (vacancy is null)
        {
            return Result.Failure(Error.NotFound("Vacancy.NotFound", $"Vacancy with ID {command.Id} was not found."));
        }

        var result = vacancy.Publish();
        if (result.IsFailure)
        {
            return result;
        }

        _vacancyRepository.Update(vacancy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
