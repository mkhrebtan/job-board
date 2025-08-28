using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.Enums;

public abstract class ResumeStatus : Enumeration<ResumeStatus>
{
    public static readonly ResumeStatus Draft = new DraftResume();
    public static readonly ResumeStatus Published = new PublishedResume();

    private ResumeStatus(string code, string name)
        : base(code, name)
    {
    }

    public abstract bool CanTransitionTo(ResumeStatus targetStatus);

    private sealed class DraftResume : ResumeStatus
    {
        internal DraftResume()
            : base("DRAFT", "Draft")
        {
        }

        public override bool CanTransitionTo(ResumeStatus targetStatus)
        {
            return targetStatus == Published;
        }
    }

    private sealed class PublishedResume : ResumeStatus
    {
        internal PublishedResume()
            : base("PUBLISHED", "Published")
        {
        }

        public override bool CanTransitionTo(ResumeStatus targetStatus)
        {
            return targetStatus == Draft;
        }
    }
}
