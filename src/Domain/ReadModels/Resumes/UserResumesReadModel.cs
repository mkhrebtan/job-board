namespace Domain.ReadModels.Resumes;

public record UserResumesReadModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ResumeId { get; set; }

    required public string Title { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}