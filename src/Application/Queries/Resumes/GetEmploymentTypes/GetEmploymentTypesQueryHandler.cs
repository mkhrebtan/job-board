using Application.Abstractions.Messaging;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Resumes.GetEmploymentTypes;

internal sealed class GetEmploymentTypesQueryHandler : IQueryHandler<GetEmploymentTypesQuery, GetEmploymentTypesQueryResponse>
{
    public async Task<Result<GetEmploymentTypesQueryResponse>> Handle(GetEmploymentTypesQuery query, CancellationToken cancellationToken)
    {
        var types = EmploymentType.List().Select(et => et.Code);
        return Result<GetEmploymentTypesQueryResponse>.Success(new GetEmploymentTypesQueryResponse(types));
    }
}
