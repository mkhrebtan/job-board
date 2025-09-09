namespace Domain.ReadModels.Resumes;

public record ResumeListingReadModel
{
    public Guid ResumeId { get; set; }

    public string Title { get; set; }

    public string FirstName { get; set; }

    public int TotalMonthsOfExperience { get; set; }

    public decimal? ExpectedSalary { get; set; }

    public string? ExpectedSalaryCurrency { get; set; }

    public List<string> EmploymentTypes { get; set; }

    public List<string> WorkArrangements { get; set; }

    public string Country { get; set; }

    public string City { get; set; }

    public string? Region { get; set; }

    public string? District { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}