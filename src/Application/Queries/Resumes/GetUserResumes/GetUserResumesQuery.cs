using Application.Abstractions.Messaging;
using Application.Services;

namespace Application.Queries.Resumes.GetUserResumes;

public record GetUserResumesQuery(Guid UserId) : IQuery<GetUserResumesResponse>;

public record GetUserResumesResponse(IEnumerable<UserResumeDto> Resumes);

public record UserResumeDto
{
    public Guid Id { get; init; }

    required public string Title { get; init; }

    public bool IsPublished { get; init; }

    public DateTime CreatedAt { get; init; }
}
