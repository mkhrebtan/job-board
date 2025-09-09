using Application.Abstractions.Messaging;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Resumes.GetAll;

public record GetAllResumesQuery() : IQuery<GetAllResumesQueryResponse>;

public record GetAllResumesQueryResponse(IEnumerable<ResumeDto> Resumes);

public record ResumeDto(
    Guid Id,
    string Title,
    string FirstName,
    int TotalMonthsOfExperience,
    decimal? ExpectedSalary,
    string? ExpectedSalaryCurrency,
    string Country,
    string City,
    string? Region,
    string? District,
    decimal? Latitude,
    decimal? Longitude,
    DateTime LastUpdatedAt);

internal sealed class GetAllResumesQueryHandler : IQueryHandler<GetAllResumesQuery, GetAllResumesQueryResponse>
{
    private readonly IResumeListingReadModelRepository _resumeListingReadModelRepository;

    public GetAllResumesQueryHandler(IResumeListingReadModelRepository resumeListingReadModelRepository)
    {
        _resumeListingReadModelRepository = resumeListingReadModelRepository;
    }

    public async Task<Result<GetAllResumesQueryResponse>> Handle(GetAllResumesQuery query, CancellationToken cancellationToken = default)
    {
        var resumes = (await _resumeListingReadModelRepository.GetAllAsync(cancellationToken))
            .Select(r => new ResumeDto(
                r.ResumeId,
                r.Title,
                r.FirstName,
                r.TotalMonthsOfExperience,
                r.ExpectedSalary,
                r.ExpectedSalaryCurrency,
                r.Country,
                r.City,
                r.Region,
                r.District,
                r.Latitude,
                r.Longitude,
                r.LastUpdatedAt));
        return Result<GetAllResumesQueryResponse>.Success(new GetAllResumesQueryResponse(resumes));
    }
}
