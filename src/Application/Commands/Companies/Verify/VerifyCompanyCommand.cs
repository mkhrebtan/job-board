using Application.Abstractions.Messaging;
using FluentValidation;

namespace Application.Commands.Companies.Verify;

public record VerifyCompanyCommand(Guid Id) : ICommand;