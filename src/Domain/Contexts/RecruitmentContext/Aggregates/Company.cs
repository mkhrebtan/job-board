using Domain.Abstraction;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Contexts.RecruitmentContext.ValueObjects;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.RecruitmentContext.Aggregates;

public class Company : AggregateRoot<CompanyId>
{
    public const int MaxNameLength = 100;

    private Company(string name, RichTextContent description, WebsiteUrl websiteUrl, LogoUrl logoUrl, int? size)
        : base(new CompanyId())
    {
        Name = name;
        Description = description;
        WebsiteUrl = websiteUrl;
        LogoUrl = logoUrl;
        Size = size;
        IsVerified = false;
    }

    public string Name { get; private set; }

    public RichTextContent Description { get; private set; }

    public WebsiteUrl WebsiteUrl { get; private set; }

    public LogoUrl LogoUrl { get; private set; }

    public int? Size { get; private set; }

    public bool IsVerified { get; private set; }

    public static Result<Company> Create(string name, RichTextContent description, WebsiteUrl websiteUrl, LogoUrl logoUrl, int? size)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Company>.Failure(new Error("Company.InvalidName", "Company name cannot be null or empty."));
        }

        if (name.Length > MaxNameLength)
        {
            return Result<Company>.Failure(new Error("Company.NameTooLong", $"Company name cannot exceed {MaxNameLength} characters."));
        }

        if (description == null)
        {
            return Result<Company>.Failure(new Error("Company.NullDescription", "Description cannot be null."));
        }

        if (websiteUrl == null)
        {
            return Result<Company>.Failure(new Error("Company.NullWebsiteUrl", "Website URL cannot be null."));
        }

        if (logoUrl == null)
        {
            return Result<Company>.Failure(new Error("Company.NullLogoUrl", "Logo URL cannot be null."));
        }

        if (size.HasValue && size <= 0)
        {
            return Result<Company>.Failure(new Error("Company.InvalidSize", "Company size must be a positive integer."));
        }

        var company = new Company(name.Trim(), description, websiteUrl, logoUrl, size);
        return Result<Company>.Success(company);
    }

    public Result Verify()
    {
        if (IsVerified)
        {
            return Result.Failure(new Error("Company.AlreadyVerified", "Company is already verified."));
        }

        IsVerified = true;
        return Result.Success();
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(new Error("Company.InvalidName", "Company name cannot be null or empty."));
        }

        Name = name.Trim();
        return Result.Success();
    }

    public Result UpdateWebsiteUrl(WebsiteUrl websiteUrl)
        => UpdateProperty(websiteUrl, url => WebsiteUrl = url, nameof(WebsiteUrl));

    public Result UpdateLogoUrl(LogoUrl logoUrl)
        => UpdateProperty(logoUrl, url => LogoUrl = url, nameof(LogoUrl));

    public Result UpdateDescription(RichTextContent description)
        => UpdateProperty(description, desc => Description = desc, nameof(Description));

    public Result UpdateSize(int? size)
    {
        if (size.HasValue && size <= 0)
        {
            return Result.Failure(new Error("Company.InvalidSize", "Company size must be a positive integer."));
        }

        Size = size;
        return Result.Success();
    }

    private static Result UpdateProperty<T>(T newValue, Action<T> updateAction, string propertyName)
    {
        if (newValue is null || newValue.Equals(default(T)))
        {
            return Result.Failure(new Error($"Vacancy.Invalid{propertyName}", $"{propertyName} cannot be null or empty."));
        }

        updateAction(newValue);
        return Result.Success();
    }
}
