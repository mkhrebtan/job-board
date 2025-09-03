using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.JobPostingContext.ValueObjects;

public class Location : ValueObject
{
    public const int MaxCountryLength = 100;
    public const int MaxCityLength = 100;
    public const int MaxRegionLength = 100;
    public const int MaxDistrictLength = 100;
    public const int MaxAddressLength = 250;

    private Location(
        string country,
        string city,
        string? region,
        string? district,
        string? address,
        decimal? latitude,
        decimal? longitude)
    {
        Country = country;
        City = city;
        Region = region;
        District = district;
        Address = address;
        Latitude = latitude;
        Longitude = longitude;
    }

    public string Country { get; private init; }

    public string City { get; private init; }

    public string? Region { get; private init; }

    public string? District { get; private init; }

    public string? Address { get; private init; }

    public decimal? Latitude { get; private init; }

    public decimal? Longitude { get; private init; }

    public static Result<Location> Create(
        string country,
        string city,
        string? region = null,
        string? district = null,
        string? address = null,
        decimal? latitude = null,
        decimal? longitude = null)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return Result<Location>.Failure(new Error("Location.InvalidCountry", "Country cannot be null or empty."));
        }

        if (country.Length > MaxCountryLength)
        {
            return Result<Location>.Failure(new Error("Location.CountryTooLong", $"Country cannot exceed {MaxCountryLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return Result<Location>.Failure(new Error("Location.InvalidCity", "City cannot be null or empty."));
        }

        if (city.Length > MaxCityLength)
        {
            return Result<Location>.Failure(new Error("Location.CityTooLong", $"City cannot exceed {MaxCityLength} characters."));
        }

        if (region is not null && region.Length > MaxRegionLength)
        {
            return Result<Location>.Failure(new Error("Location.RegionTooLong", $"Region cannot exceed {MaxRegionLength} characters."));
        }

        if (district is not null && district.Length > MaxDistrictLength)
        {
            return Result<Location>.Failure(new Error("Location.DistrictTooLong", $"District cannot exceed {MaxDistrictLength} characters."));
        }

        if (address is not null && address.Length > MaxAddressLength)
        {
            return Result<Location>.Failure(new Error("Location.AddressTooLong", $"Address cannot exceed {MaxAddressLength} characters."));
        }

        if (latitude is not null && (latitude < -90 || latitude > 90))
        {
            return Result<Location>.Failure(new Error("Location.InvalidLatitude", "Latitude must be between -90 and 90."));
        }

        if (longitude is not null && (longitude < -180 || longitude > 180))
        {
            return Result<Location>.Failure(new Error("Location.InvalidLongitude", "Longitude must be between -180 and 180."));
        }

        var location = new Location(
            country.Trim(),
            city.Trim(),
            region?.Trim(),
            district?.Trim(),
            address?.Trim(),
            latitude,
            longitude);
        return Result<Location>.Success(location);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Country;
        yield return City;
        yield return Region ?? string.Empty;
        yield return District ?? string.Empty;
        yield return Address ?? string.Empty;
        yield return Latitude ?? 0;
        yield return Longitude ?? 0;
    }
}
