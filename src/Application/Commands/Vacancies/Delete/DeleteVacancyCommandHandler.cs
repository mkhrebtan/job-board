using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Repos.Vacancies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Vacancies.Delete;

internal sealed class DeleteVacancyCommandHandler : ICommandHandler<DeleteVacancyCommand>
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVacancyCommandHandler(IVacancyRepository vacancyRepository, IUnitOfWork unitOfWork)
    {
        _vacancyRepository = vacancyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteVacancyCommand command, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(new VacancyId(command.Id), cancellationToken);
        if (vacancy is null)
        {
            return Result.Failure(Error.NotFound("Vacancy.NotFound", $"Vacancy with ID {command.Id} was not found."));
        }

        _vacancyRepository.Remove(vacancy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
