using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Repos.Users;
using Domain.Services;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Users.Register.JobSeeker;

internal sealed class RegisterJobSeekerCommandHandler : ICommandHandler<RegisterJobSeekerCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterJobSeekerCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userService = new UserService(_userRepository);
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RegisterJobSeekerCommand command, CancellationToken cancellationToken = default)
    {
        if (!Helpers.TryCreateVO(() => Email.Create(command.Email), out Email email, out Error error))
        {
            return Result.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => PhoneNumber.Create(command.PhoneNumber, command.PhoneNumberRegionCode), out PhoneNumber phoneNumber, out error))
        {
            return Result.Failure(error);
        }

        var userResult = await _userService.CreateUserAsync(
            command.FirstName,
            command.LastName,
            email: email,
            phoneNumber: phoneNumber,
            password: command.Password,
            role: command.UserRole,
            passwordHasher: _passwordHasher,
            ct: cancellationToken);
        if (userResult.IsFailure)
        {
            return Result.Failure(userResult.Error);
        }

        _userRepository.Add(userResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}