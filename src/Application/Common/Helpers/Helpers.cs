using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Application.Common.Helpers;

internal static class Helpers
{
    internal static bool TryCreateVO<TValue>(Func<Result<TValue>> factory, out TValue value, out Error error)
        where TValue : ValueObject
    {
        var result = factory();
        if (result.IsFailure)
        {
            error = result.Error;
            value = default!;
            return false;
        }

        error = Error.None;
        value = result.Value;
        return true;
    }

    internal static bool TryParseSmartEnum<TEnum>(ICollection<string> enumCodes, string errorCode, string errorMessage, out ICollection<TEnum> enums, out Error error)
        where TEnum : Enumeration<TEnum>
    {
        enums = new List<TEnum>(enumCodes.Count);
        foreach (var arrangement in enumCodes)
        {
            var parseResult = Enumeration<TEnum>.FromCode(arrangement);
            if (parseResult is null)
            {
                enums = default!;
                error = Error.Problem(errorCode, $"{errorMessage}: '{arrangement}'.");
                return false;
            }

            enums.Add(parseResult);
        }

        error = Error.None;
        return true;
    }
}
