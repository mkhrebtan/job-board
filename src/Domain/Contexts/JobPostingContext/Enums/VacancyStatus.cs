using Domain.Abstraction;

namespace Domain.Contexts.JobPostingContext.Enums;

public abstract class VacancyStatus : Enumeration<VacancyStatus>
{
    public static readonly VacancyStatus Draft = new DraftVacancyStatus();
    public static readonly VacancyStatus Registered = new RegisteredVacancyStatus();
    public static readonly VacancyStatus Published = new PublishedVacancyStatus();
    public static readonly VacancyStatus Archived = new ArchivedVacancyStatus();

    private VacancyStatus(string code, string name)
        : base(code, name)
    {
    }

    public abstract bool IsEditable { get; }

    public abstract bool CanTransitionTo(VacancyStatus targetStatus);

    private sealed class DraftVacancyStatus : VacancyStatus
    {
        internal DraftVacancyStatus()
            : base("DRAFT", "Draft")
        {
        }

        public override bool IsEditable => true;

        public override bool CanTransitionTo(VacancyStatus targetStatus)
        {
            return targetStatus == Registered;
        }
    }

    private sealed class RegisteredVacancyStatus : VacancyStatus
    {
        internal RegisteredVacancyStatus()
            : base("REGISTERED", "Registered")
        {
        }

        public override bool IsEditable => false;

        public override bool CanTransitionTo(VacancyStatus targetStatus)
        {
            return targetStatus == Published;
        }
    }

    private sealed class PublishedVacancyStatus : VacancyStatus
    {
        internal PublishedVacancyStatus()
            : base("PUBLISHED", "Published")
        {
        }

        public override bool IsEditable => true;

        public override bool CanTransitionTo(VacancyStatus targetStatus)
        {
            return targetStatus == Archived;
        }
    }

    private sealed class ArchivedVacancyStatus : VacancyStatus
    {
        internal ArchivedVacancyStatus()
            : base("ARCHIVED", "Archived")
        {
        }

        public override bool IsEditable => false;

        public override bool CanTransitionTo(VacancyStatus targetStatus)
        {
            return targetStatus == Published;
        }
    }
}
