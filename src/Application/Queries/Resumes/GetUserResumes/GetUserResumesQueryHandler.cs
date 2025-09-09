using Application.Abstractions.Messaging;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Resumes.GetUserResumes;

internal sealed class GetUserResumesQueryHandler : IQueryHandler<GetUserResumesQuery, GetUserResumesResponse>
{
    private readonly IUserResumesReadModelRepository _userResumesReadModelRepository;

    public GetUserResumesQueryHandler(IUserResumesReadModelRepository userResumesReadModelRepository)
    {
        _userResumesReadModelRepository = userResumesReadModelRepository;
    }

    public async Task<Result<GetUserResumesResponse>> Handle(GetUserResumesQuery query, CancellationToken cancellationToken = default)
    {
        var resumes = await _userResumesReadModelRepository.GetByUserIdAsync(query.UserId);
        var response = new GetUserResumesResponse(resumes.Select(r => new UserResumeDto
        {
            Id = r.ResumeId,
            Title = r.Title,
            IsPublished = r.IsPublished,
            CreatedAt = r.CreatedAtUtc
        }));
        return Result<GetUserResumesResponse>.Success(response);
    }
}