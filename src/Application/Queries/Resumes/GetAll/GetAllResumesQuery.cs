using Application.Abstractions.Messaging;
using Domain.ReadModels;

namespace Application.Queries.Resumes.GetAll;

public record GetAllResumesQuery(
    string? Search,
    string? SortProperty,
    bool? SortDescending,
    bool? OnlyWithSalary,
    bool? NoExperience,
    decimal? MinSalary,
    decimal? MaxSalary,
    string? Currency,
    string? Country,
    string? City,
    string? Region,
    string? District,
    decimal? Latitude,
    decimal? Longitude,
    int PageNumber,
    int PageSize,
    List<string>? EmploymentTypes,
    List<string>? WorkArrangements) : IQuery<GetAllResumesQueryResponse>;

public record GetAllResumesQueryResponse(IPagedList<ResumeDto> Resumes);

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