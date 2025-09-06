using Application.Abstractions.Messaging;

namespace Application.Commands.VacancyApplications.ApplyWithFile;

public record ApplyToVacancyWithFileCommand(Guid UserId, Guid VacancyId, string? CoverLetterContent, string FileUrl) : ICommand<ApplyToVacancyCommandResponse>;

public record ApplyToVacancyCommandResponse(Guid Id);