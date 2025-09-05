using Application.Abstractions.Messaging;

namespace Application.Commands.Vacancies.UpdateRectuiterInfo;

public record UpdateVacancyRecruiterInfo(
    Guid Id,
    string RecruiterFirstName,
    string RecruiterEmail,
    string RecruiterPhoneNumber,
    string RecruiterPhoneNumberRegionCode) : ICommand;
