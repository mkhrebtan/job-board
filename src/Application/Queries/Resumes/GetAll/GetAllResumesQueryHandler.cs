using System.Linq.Expressions;
using Application.Abstractions.Messaging;
using Domain.ReadModels;
using Domain.ReadModels.Resumes;
using Domain.Repos.ReadModels;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Resumes.GetAll;

internal sealed class GetAllResumesQueryHandler : IQueryHandler<GetAllResumesQuery, GetAllResumesQueryResponse>
{
    private readonly IResumeListingReadModelRepository _resumeListingReadModelRepository;
    private readonly IPagedList<ResumeDto> _pagedList;

    public GetAllResumesQueryHandler(IResumeListingReadModelRepository resumeListingReadModelRepository, IPagedList<ResumeDto> pagedList)
    {
        _resumeListingReadModelRepository = resumeListingReadModelRepository;
        _pagedList = pagedList;
    }

    public async Task<Result<GetAllResumesQueryResponse>> Handle(GetAllResumesQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<ResumeListingReadModel> resumesQuery = _resumeListingReadModelRepository.GetResumesQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            resumesQuery = resumesQuery.Where(r => r.Title.Contains(query.Search));
        }

        Expression<Func<ResumeListingReadModel, object>> sortExpression = query.SortProperty?.ToLower() switch
        {
            "totalmonthsofexperience" => r => r.TotalMonthsOfExperience,
            "expectedsalary" => r => r.ExpectedSalary ?? 0m,
            "lastupdatedat" => r => r.LastUpdatedAt,
            _ => r => r.ResumeId,
        };

        if (query.SortDescending == true)
        {
            resumesQuery = resumesQuery.OrderByDescending(sortExpression);
        }
        else
        {
            resumesQuery = resumesQuery.OrderBy(sortExpression);
        }

        if (query.OnlyWithSalary == true)
        {
            resumesQuery = resumesQuery.Where(r => r.ExpectedSalary.HasValue);
        }

        if (query.NoExperience == true)
        {
            resumesQuery = resumesQuery.Where(r => r.TotalMonthsOfExperience == 0);
        }

        if (query.MinSalary.HasValue)
        {
            if (string.IsNullOrEmpty(query.Currency))
            {
                return Result<GetAllResumesQueryResponse>.Failure(Error.Problem("GetResumes.EmptyCurrency", "Currency must be provided when filtering salary."));
            }

            resumesQuery = resumesQuery.Where(r => r.ExpectedSalary >= query.MinSalary.Value && r.ExpectedSalaryCurrency == query.Currency);
        }

        if (query.MaxSalary.HasValue)
        {
            if (string.IsNullOrEmpty(query.Currency))
            {
                return Result<GetAllResumesQueryResponse>.Failure(Error.Problem("GetResumes.EmptyCurrency", "Currency must be provided when filtering salary."));
            }

            resumesQuery = resumesQuery.Where(r => r.ExpectedSalary <= query.MaxSalary.Value && r.ExpectedSalaryCurrency == query.Currency);
        }

        if (!string.IsNullOrWhiteSpace(query.Country))
        {
            resumesQuery = resumesQuery.Where(r => r.Country == query.Country);

            if (!string.IsNullOrWhiteSpace(query.City))
            {
                IQueryable<ResumeListingReadModel>? resumesAditionalCities = default!;
                if (query.Latitude.HasValue && query.Longitude.HasValue)
                {
                    const decimal radius = 0.2m;
                    resumesAditionalCities = resumesQuery.Where(r =>
                        r.Latitude.HasValue && r.Longitude.HasValue &&
                        Math.Abs(r.Latitude.Value - query.Latitude.Value) <= radius &&
                        Math.Abs(r.Longitude.Value - query.Longitude.Value) <= radius);
                }

                resumesQuery = resumesQuery.Where(r => r.City == query.City).Union(resumesAditionalCities);

                if (!string.IsNullOrWhiteSpace(query.Region))
                {
                    resumesQuery = resumesQuery.Where(r => r.Region == query.Region);

                    if (!string.IsNullOrWhiteSpace(query.District))
                    {
                        resumesQuery = resumesQuery.Where(r => r.District == query.District);
                    }
                }
            }
        }

        if (query.EmploymentTypes != null && query.EmploymentTypes.Any())
        {
            resumesQuery = resumesQuery.Where(r => r.EmploymentTypes.Intersect(query.EmploymentTypes).Any());
        }

        if (query.WorkArrangements != null && query.WorkArrangements.Any())
        {
            resumesQuery = resumesQuery.Where(r => r.WorkArrangements.Intersect(query.WorkArrangements).Any());
        }

        var resumesDtos = resumesQuery
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

        var resumes = await _pagedList.Create(resumesDtos, query.PageNumber, query.PageSize);
        return Result<GetAllResumesQueryResponse>.Success(new GetAllResumesQueryResponse(resumes));
    }
}
