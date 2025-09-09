namespace Domain.ReadModels.Vacancies;

public record VacancyApplicationsReadModel
{
    public Guid VacancyApplicationId { get; set; }

    public Guid VacancyId { get; set; }

    public string SeekerFirstName { get; set; }

    public string SeekerLastName { get; set; }

    public string CoverLetter { get; set; }

    public Guid? ResumeId { get; set; }

    public string? ResumeTitle { get; set; }

    public string? FileUrl { get; set; }

    public DateTime AppliedAt { get; set; }
}
