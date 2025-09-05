using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Repos.Vacancies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Vacancies.UpdateTitle;

internal sealed class UpdateVacancyTitleCommandHandler : ICommandHandler<UpdateVacancyTitleCommand>
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVacancyTitleCommandHandler(IVacancyRepository vacancyRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _vacancyRepository = vacancyRepository;
    }

    public async Task<Result> Handle(UpdateVacancyTitleCommand command, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(command.Id, cancellationToken);
        if (vacancy is null)
        {
            return Result.Failure(Error.NotFound("Vacancy.NotFound", $"Vacancy with ID {command.Id} was not found."));
        }

        var titleResult = VacancyTitle.Create(command.Title);
        if (titleResult.IsFailure)
        {
            return Result.Failure(titleResult.Error);
        }

        vacancy.UpdateTitle(titleResult.Value);
        _vacancyRepository.Update(vacancy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}