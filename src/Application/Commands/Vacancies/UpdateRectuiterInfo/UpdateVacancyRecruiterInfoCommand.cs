using Application.Abstractions.Messaging;

namespace Application.Commands.Vacancies.UpdateRectuiterInfo;

public record UpdateVacancyRecruiterInfoCommand(
    Guid Id,
    string RecruiterFirstName,
    string RecruiterEmail,
    string RecruiterPhoneNumber,
    string RecruiterPhoneNumberRegionCode) : ICommand;
