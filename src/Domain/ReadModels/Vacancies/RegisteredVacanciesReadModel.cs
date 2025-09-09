namespace Domain.ReadModels.Vacancies;

public record RegisteredVacanciesReadModel(
    Guid VacancyId,
    string Title,
    string CompanyName,
    string UserFullName,
    string UserEmail,
    string UserPhoneNumber,
    DateTime RegisteredAt);