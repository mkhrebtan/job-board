using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Repos.Vacancies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Vacancies.Archive;

internal sealed class ArchiveVacancyCommandHandler : ICommandHandler<ArchiveVacancyCommand>
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveVacancyCommandHandler(IVacancyRepository vacancyRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _vacancyRepository = vacancyRepository;
    }

    public async Task<Result> Handle(ArchiveVacancyCommand command, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(command.Id, cancellationToken);
        if (vacancy is null)
        {
            return Result.Failure(Error.NotFound("Vacancy.NotFound", "The vacancy was not found."));
        }

        var result = vacancy.Archive();
        if (result.IsFailure)
        {
            return result;
        }

        _vacancyRepository.Update(vacancy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
