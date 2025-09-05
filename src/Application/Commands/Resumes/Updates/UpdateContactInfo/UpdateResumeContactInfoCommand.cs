using Application.Abstractions.Messaging;

namespace Application.Commands.Resumes.Updates.UpdateContactInfo;

public record UpdateResumeContactInfoCommand(
    Guid Id,
    string Email,
    string PhoneNumber,
    string PhoneNumberRegionCode) : ICommand;