using Application.Abstractions.Messaging;

namespace Application.Commands.Vacancies.UpdateLocation;

public record UpdateVacancyLocationCommand(
    Guid Id,
    string Country,
    string City,
    string? Region,
    string? District,
    string? Address,
    decimal? Latitude,
    decimal? Longitude) : ICommand;
