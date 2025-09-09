using Application.Abstractions.Messaging;

namespace Application.Queries.Resumes.GetWorkArrangements;

public record GetWorkArrangementsQuery : IQuery<GetWorkArrangementsQueryResponse>;

public record GetWorkArrangementsQueryResponse(IEnumerable<string> WorkArrangements);