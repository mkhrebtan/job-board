using Application.Abstractions.Messaging;
using Application.Commands.VacancyApplications.ApplyWithFile;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ApplicationContext.ValueObjects;
using Domain.Repos.Resumes;
using Domain.Repos.Users;
using Domain.Repos.Vacancies;
using Domain.Repos.VacancyApplications;
using Domain.Services;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.VacancyApplications.ApplyWithResume;

internal sealed class ApplyToVacancyWithResumeCommandHandler : ICommandHandler<ApplyToVacancyWithResumeCommand, ApplyToVacancyCommandResponse>
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IResumeRepository _resumeRepository;
    private readonly VacancyApplicationService _vacancyApplicationService;
    private readonly IVacancyApplicationRepository _vacancyApplicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApplyToVacancyWithResumeCommandHandler(
        IVacancyRepository vacancyRepository,
        IUserRepository userRepository,
        VacancyApplicationService vacancyApplicationService,
        IVacancyApplicationRepository vacancyApplicationRepository,
        IUnitOfWork unitOfWork)
    {
        _vacancyRepository = vacancyRepository;
        _userRepository = userRepository;
        _vacancyApplicationService = vacancyApplicationService;
        _vacancyApplicationRepository = vacancyApplicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ApplyToVacancyCommandResponse>> Handle(ApplyToVacancyWithResumeCommand command, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(command.VacancyId, cancellationToken);
        if (vacancy is null)
        {
            return Result<ApplyToVacancyCommandResponse>.Failure(Error.NotFound("ApplyToVacancyWithFileCommandHandler.VacancyNotFound", $"Vacancy was not found."));
        }

        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return Result<ApplyToVacancyCommandResponse>.Failure(Error.NotFound("ApplyToVacancyWithFileCommandHandler.UserNotFound", $"User was not found."));
        }

        var resume = await _resumeRepository.GetByIdAsync(command.ResumeId, cancellationToken);
        if (resume is null || resume.SeekerId != user.Id)
        {
            return Result<ApplyToVacancyCommandResponse>.Failure(Error.NotFound("ApplyToVacancyWithFileCommandHandler.ResumeNotFound", $"Resume was not found."));
        }

        if (!Helpers.TryCreateVO(() => CoverLetter.Create(command.CoverLetterContent ?? string.Empty), out CoverLetter coverLetter, out Error error))
        {
            return Result<ApplyToVacancyCommandResponse>.Failure(error);
        }

        var applicationResult = await _vacancyApplicationService.ApplyToVacancyWithCreatedResumeAsync(user, vacancy, coverLetter, resume);
        if (applicationResult.IsFailure)
        {
            return Result<ApplyToVacancyCommandResponse>.Failure(applicationResult.Error);
        }

        _vacancyApplicationRepository.Add(applicationResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<ApplyToVacancyCommandResponse>.Success(new ApplyToVacancyCommandResponse(applicationResult.Value.Id.Value));
    }
}