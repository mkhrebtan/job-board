using Application.Abstractions.Messaging;
using Domain.ReadModels;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Resumes.GetUserResumes;

internal sealed class GetUserResumesQueryHandler : IQueryHandler<GetUserResumesQuery, GetUserResumesResponse>
{
    private readonly IUserResumesReadModelRepository _userResumesReadModelRepository;
    private readonly IPagedList<UserResumeDto> _pagedList;

    public GetUserResumesQueryHandler(IUserResumesReadModelRepository userResumesReadModelRepository, IPagedList<UserResumeDto> pagedList)
    {
        _userResumesReadModelRepository = userResumesReadModelRepository;
        _pagedList = pagedList;
    }

    public async Task<Result<GetUserResumesResponse>> Handle(GetUserResumesQuery query, CancellationToken cancellationToken = default)
    {
        var resumesQuery = _userResumesReadModelRepository.GetUserResumesQueryable(query.UserId);
        if (query.NewFirst == true)
        {
            resumesQuery = resumesQuery.OrderByDescending(r => r.CreatedAtUtc);
        }
        else
        {
            resumesQuery = resumesQuery.OrderBy(r => r.CreatedAtUtc);
        }

        var resumesDtos = resumesQuery.Select(r => new UserResumeDto
        {
            Id = r.ResumeId,
            Title = r.Title,
            IsPublished = r.IsPublished,
            CreatedAt = r.CreatedAtUtc
        });

        var vacancies = await _pagedList.Create(resumesDtos, query.Page, query.PageSize);

        return Result<GetUserResumesResponse>.Success(new GetUserResumesResponse(vacancies));
    }
}