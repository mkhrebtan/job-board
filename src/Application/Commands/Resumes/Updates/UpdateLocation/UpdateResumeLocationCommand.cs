using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Create;

namespace Application.Commands.Resumes.Updates.UpdateLocation;

public record UpdateResumeLocationCommand(
    Guid Id,
    string Country,
    string City,
    string? Region,
    string? District,
    decimal? Latitude,
    decimal? Longitude) : ICommand;