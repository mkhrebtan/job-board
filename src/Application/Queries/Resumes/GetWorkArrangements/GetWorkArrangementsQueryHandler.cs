using Application.Abstractions.Messaging;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Resumes.GetWorkArrangements;

internal sealed class GetWorkArrangementsQueryHandler : IQueryHandler<GetWorkArrangementsQuery, GetWorkArrangementsQueryResponse>
{
    public async Task<Result<GetWorkArrangementsQueryResponse>> Handle(GetWorkArrangementsQuery query, CancellationToken cancellationToken = default)
    {
        var workArrangements = WorkArrangement.List().Select(wa => wa.Code);
        return Result<GetWorkArrangementsQueryResponse>.Success(new GetWorkArrangementsQueryResponse(workArrangements));
    }
}