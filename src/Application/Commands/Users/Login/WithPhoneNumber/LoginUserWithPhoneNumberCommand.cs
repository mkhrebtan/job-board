using Application.Abstractions.Messaging;
using Application.Commands.Users.Login.WithEmail;

namespace Application.Commands.Users.Login.WithPhoneNumber;

public record LoginUserWithPhoneNumberCommand(string PhoneNumber, string PhoneNumberRegionCode, string Password) : ICommand<LoginUserCommandResponse>;
