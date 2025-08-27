using System.Reflection;

namespace Domain.Abstraction;

public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>
    where TEnum : Enumeration<TEnum>
{
    private static readonly Dictionary<string, TEnum> Enumerations = CreateEnumerations();

    protected Enumeration(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; private init; }

    public string Name { get; private init; }

    public static TEnum? FromCode(string code)
    {
        return Enumerations.TryGetValue(code, out TEnum? value) ? value : default;
    }

    public static TEnum? FromName(string name)
    {
        return Enumerations.Values.SingleOrDefault(e => e.Name == name);
    }

    public bool Equals(Enumeration<TEnum>? other)
    {
        return other is not null &&
            GetType() == other.GetType() &&
            Code == other.Code;
    }

    public override bool Equals(object? obj)
    {
        return obj is Enumeration<TEnum> other &&
            Equals(other);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

    private static Dictionary<string, TEnum> CreateEnumerations()
    {
        var enumerationType = typeof(TEnum);
        var fields = enumerationType
            .GetFields(
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy)
            .Where(fieldInfo => enumerationType.IsAssignableFrom(fieldInfo.FieldType))
            .Select(fieldInfo => (TEnum)fieldInfo.GetValue(default)!);

        return fields.ToDictionary(e => e.Code);
    }
}
