namespace API.Endpoints.Users.RegisterJobSeeker;

internal sealed record RegisterUserRequest(
       string FirstName,
       string LastName,
       string Email,
       string PhoneNumber,
       string PhoneNumberRegionCode,
       string Password);