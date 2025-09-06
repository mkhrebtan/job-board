using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Repos.Vacancies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Vacancies.UpdateLocation;

internal sealed class UpdateVacancyLocationCommandHandler : ICommandHandler<UpdateVacancyLocationCommand>
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVacancyLocationCommandHandler(IVacancyRepository vacancyRepository, IUnitOfWork unitOfWork)
    {
        _vacancyRepository = vacancyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateVacancyLocationCommand command, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(new VacancyId(command.Id), cancellationToken);
        if (vacancy is null)
        {
            return Result.Failure(Error.NotFound("Vacancy.NotFound", $"Vacancy with ID {command.Id} was not found."));
        }

        var locationResult = Location.Create(
            command.Country,
            command.City,
            command.Region,
            command.District,
            command.Address,
            command.Latitude,
            command.Longitude);
        if (locationResult.IsFailure)
        {
            return Result.Failure(locationResult.Error);
        }

        vacancy.UpdateLocation(locationResult.Value);
        _vacancyRepository.Update(vacancy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
