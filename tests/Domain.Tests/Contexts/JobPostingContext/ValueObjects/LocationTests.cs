using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Shared;

namespace Domain.Tests.Contexts.JobPostingContext.ValueObjects;

public class LocationTests
{
    [Fact]
    public void Create_WithValidCountryAndCity_ShouldReturnSuccess()
    {
        var country = "USA";
        var city = "New York";

        var result = Location.Create(country, city);

        Assert.True(result.IsSuccess);
        Assert.Equal(country, result.Value.Country);
        Assert.Equal(city, result.Value.City);
        Assert.Null(result.Value.Region);
        Assert.Null(result.Value.District);
        Assert.Null(result.Value.Address);
        Assert.Null(result.Value.Latitude);
        Assert.Null(result.Value.Longitude);
    }

    [Fact]
    public void Create_WithAllFields_ShouldReturnSuccess()
    {
        var country = "USA";
        var city = "New York";
        var region = "NY";
        var district = "Manhattan";
        var address = "123 Main St";
        var latitude = 40.7128m;
        var longitude = -74.0060m;

        var result = Location.Create(country, city, region, district, address, latitude, longitude);

        Assert.True(result.IsSuccess);
        Assert.Equal(country, result.Value.Country);
        Assert.Equal(city, result.Value.City);
        Assert.Equal(region, result.Value.Region);
        Assert.Equal(district, result.Value.District);
        Assert.Equal(address, result.Value.Address);
        Assert.Equal(latitude, result.Value.Latitude);
        Assert.Equal(longitude, result.Value.Longitude);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithInvalidCountry_ShouldReturnFailure(string country)
    {
        var result = Location.Create(country, "New York");

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithInvalidCity_ShouldReturnFailure(string city)
    {
        var result = Location.Create("USA", city);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithLatitudeBelowMinimum_ShouldReturnFailure()
    {
        var result = Location.Create("USA", "New York", latitude: -91m);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithLatitudeAboveMaximum_ShouldReturnFailure()
    {
        var result = Location.Create("USA", "New York", latitude: 91m);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithLongitudeBelowMinimum_ShouldReturnFailure()
    {
        var result = Location.Create("USA", "New York", longitude: -181m);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithLongitudeAboveMaximum_ShouldReturnFailure()
    {
        var result = Location.Create("USA", "New York", longitude: 181m);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithLatitudeAtBoundaries_ShouldReturnSuccess()
    {
        var resultMin = Location.Create("USA", "New York", latitude: -90m);
        var resultMax = Location.Create("USA", "New York", latitude: 90m);

        Assert.True(resultMin.IsSuccess);
        Assert.Equal(-90m, resultMin.Value.Latitude);
        Assert.True(resultMax.IsSuccess);
        Assert.Equal(90m, resultMax.Value.Latitude);
    }

    [Fact]
    public void Create_WithLongitudeAtBoundaries_ShouldReturnSuccess()
    {
        var resultMin = Location.Create("USA", "New York", longitude: -180m);
        var resultMax = Location.Create("USA", "New York", longitude: 180m);

        Assert.True(resultMin.IsSuccess);
        Assert.Equal(-180m, resultMin.Value.Longitude);
        Assert.True(resultMax.IsSuccess);
        Assert.Equal(180m, resultMax.Value.Longitude);
    }

    [Fact]
    public void Create_WithWhitespaceInFields_ShouldReturnTrimmedValues()
    {
        var result = Location.Create("  USA  ", "  New York  ", "  NY  ", "  Manhattan  ", "  123 Main St  ");

        Assert.True(result.IsSuccess);
        Assert.Equal("USA", result.Value.Country);
        Assert.Equal("New York", result.Value.City);
        Assert.Equal("NY", result.Value.Region);
        Assert.Equal("Manhattan", result.Value.District);
        Assert.Equal("123 Main St", result.Value.Address);
    }

    [Fact]
    public void Create_WithCountryExceedingMaxLength_ShouldReturnFailure()
    {
        var country = new string('A', Location.MaxCountryLength + 1);

        var result = Location.Create(country, "New York");

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithCityExceedingMaxLength_ShouldReturnFailure()
    {
        var city = new string('B', Location.MaxCityLength + 1);

        var result = Location.Create("USA", city);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithRegionExceedingMaxLength_ShouldReturnFailure()
    {
        var region = new string('C', Location.MaxRegionLength + 1);

        var result = Location.Create("USA", "New York", region);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithDistrictExceedingMaxLength_ShouldReturnFailure()
    {
        var district = new string('D', Location.MaxDistrictLength + 1);

        var result = Location.Create("USA", "New York", "NY", district);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithAddressExceedingMaxLength_ShouldReturnFailure()
    {
        var address = new string('E', Location.MaxAddressLength + 1);

        var result = Location.Create("USA", "New York", "NY", "Manhattan", address);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        var location1 = Location.Create("USA", "New York", "NY", "Manhattan", "123 Main St", 40.7128m, -74.0060m).Value;
        var location2 = Location.Create("USA", "New York", "NY", "Manhattan", "123 Main St", 40.7128m, -74.0060m).Value;

        Assert.True(location1.Equals(location2));
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        var location1 = Location.Create("USA", "New York").Value;
        var location2 = Location.Create("USA", "Boston").Value;

        Assert.False(location1.Equals(location2));
    }

    [Fact]
    public void Equals_WithNullOptionalFields_ShouldReturnTrue()
    {
        var location1 = Location.Create("USA", "New York").Value;
        var location2 = Location.Create("USA", "New York").Value;

        Assert.True(location1.Equals(location2));
    }
}
