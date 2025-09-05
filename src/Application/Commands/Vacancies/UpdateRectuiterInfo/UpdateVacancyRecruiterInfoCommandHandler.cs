using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Repos.Vacancies;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Vacancies.UpdateRectuiterInfo;

internal sealed class UpdateVacancyRecruiterInfoCommandHandler : ICommandHandler<UpdateVacancyRecruiterInfo>
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVacancyRecruiterInfoCommandHandler(IVacancyRepository vacancyRepository, IUnitOfWork unitOfWork)
    {
        _vacancyRepository = vacancyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateVacancyRecruiterInfo command, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(command.Id, cancellationToken);
        if (vacancy is null)
        {
            return Result.Failure(Error.NotFound("Vacancy.NotFound", $"Vacancy with id {command.Id} was not found."));
        }

        var emailResult = Email.Create(command.RecruiterEmail);
        if (emailResult.IsFailure)
        {
            return Result.Failure(emailResult.Error);
        }

        var phoneNumberResult = PhoneNumber.Create(command.RecruiterPhoneNumber, command.RecruiterPhoneNumberRegionCode);
        if (phoneNumberResult.IsFailure)
        {
            return Result.Failure(phoneNumberResult.Error);
        }

        var recruiterInfoResult = RecruiterInfo.Create(
            command.RecruiterFirstName,
            emailResult.Value,
            phoneNumberResult.Value);
        if (recruiterInfoResult.IsFailure)
        {
            return Result.Failure(recruiterInfoResult.Error);
        }

        vacancy.UpdateRecruiterInfo(recruiterInfoResult.Value);
        _vacancyRepository.Update(vacancy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
