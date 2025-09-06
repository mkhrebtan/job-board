using Application.Abstractions.Messaging;

namespace Application.Commands.VacancyApplications.ApplyWithFile;

public record ApplyToVacancyWithFileCommand(IApplicationInfo ApplicationInfo, Guid VacancyId, string? CoverLetterContent, string FileUrl) : ICommand<ApplyToVacancyCommandResponse>;

public record ApplyToVacancyCommandResponse(Guid Id);