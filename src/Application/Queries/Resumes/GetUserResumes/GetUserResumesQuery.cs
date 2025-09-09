using Application.Abstractions.Messaging;
using Application.Services;
using Domain.ReadModels;

namespace Application.Queries.Resumes.GetUserResumes;

public record GetUserResumesQuery(Guid UserId, bool? NewFirst, int Page, int PageSize) : IQuery<GetUserResumesResponse>;

public record GetUserResumesResponse(IPagedList<UserResumeDto> Resumes);

public record UserResumeDto
{
    public Guid Id { get; init; }

    required public string Title { get; init; }

    public bool IsPublished { get; init; }

    public DateTime CreatedAt { get; init; }
}
