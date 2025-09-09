using Application.Abstractions.Messaging;

namespace Application.Queries.Resumes.GetEmploymentTypes;

public record GetEmploymentTypesQuery() : IQuery<GetEmploymentTypesQueryResponse>;

public record GetEmploymentTypesQueryResponse(IEnumerable<string> EmploymentTypes);