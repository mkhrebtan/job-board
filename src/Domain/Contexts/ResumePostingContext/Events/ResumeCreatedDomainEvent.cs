using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Shared.Events;

namespace Domain.Contexts.ResumePostingContext.Events;

public sealed record ResumeCreatedDomainEvent(ResumeId ResumeId) : IDomainEvent;
