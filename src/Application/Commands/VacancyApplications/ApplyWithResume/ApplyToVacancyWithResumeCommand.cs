using Application.Abstractions.Messaging;
using Application.Commands.VacancyApplications.ApplyWithFile;

namespace Application.Commands.VacancyApplications.ApplyWithResume;

public record ApplyToVacancyWithResumeCommand(IApplicationInfo ApplicationInfo, Guid VacancyId, string? CoverLetterContent, Guid ResumeId) : ICommand<ApplyToVacancyCommandResponse>;
