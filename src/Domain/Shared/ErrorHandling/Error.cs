namespace Domain.Shared.ErrorHandling;

public sealed partial record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}