using Application.Abstractions.Messaging;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Repos.Vacancies;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Vacancies.GetVacancy;

internal sealed class GetVacancyQueryHandler : IQueryHandler<GetVacancyQuery, GetVacancyQueryResponse>
{
    private readonly IVacancyRepository _vacancyRepository;

    public GetVacancyQueryHandler(IVacancyRepository vacancyRepository)
    {
        _vacancyRepository = vacancyRepository;
    }

    public async Task<Result<GetVacancyQueryResponse>> Handle(GetVacancyQuery query, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(new VacancyId(query.VacancyId), cancellationToken);
        if (vacancy == null)
        {
            return Result<GetVacancyQueryResponse>.Failure(Error.NotFound("Vacancy.NotFound", "The vacancy was not found"));
        }

        var response = new GetVacancyQueryResponse(
            vacancy.Id.Value,
            vacancy.Title.Value,
            vacancy.Description.Markdown,
            vacancy.Salary.MinAmount,
            vacancy.Salary.MaxAmount,
            vacancy.Salary.Currency,
            vacancy.Location.Country,
            vacancy.Location.City,
            vacancy.Location.Region,
            vacancy.Location.District,
            vacancy.Location.Address,
            vacancy.Location.Latitude,
            vacancy.Location.Longitude,
            vacancy.RecruiterInfo.FirstName,
            vacancy.RecruiterInfo.Email.Address,
            vacancy.RecruiterInfo.PhoneNumber.Number,
            vacancy.Status.Code);

        return Result<GetVacancyQueryResponse>.Success(response);
    }
}
