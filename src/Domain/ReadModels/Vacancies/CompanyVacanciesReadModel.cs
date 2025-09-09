namespace Domain.ReadModels.Vacancies;

public record CompanyVacanciesReadModel()
{
    public Guid CompanyId { get; set; }

    public Guid VacancyId { get; set; }

    public Guid? CategoryId { get; set; }

    public string Title { get; set; }

    public decimal? SalaryFrom { get; set; }

    public decimal? SalaryTo { get; set; }

    public string? SalaryCurrency { get; set; }

    public string Country { get; set; }

    public string City { get; set; }

    public string? Region { get; set; }

    public string? District { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public string Status { get; set; }
}