using Application.Abstractions.Messaging;
using Domain.Shared.ErrorHandling;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using static Application.Abstraction.Behaviour.ValidationDecorator;

namespace Application.Tests.Behaviours.Validation;

public class CommandHandlerTests
{
    private readonly Mock<ICommandHandler<BehaviourTestsData.DummyCommandWithResponse, BehaviourTestsData.DummyResponse>> _innerHandlerMock = new();
    private readonly Mock<IValidator<BehaviourTestsData.DummyCommandWithResponse>> _validatorMock = new();
    private readonly CommandHandler<BehaviourTestsData.DummyCommandWithResponse, BehaviourTestsData.DummyResponse> _handler;

    public CommandHandlerTests()
    {
        _handler = new CommandHandler<BehaviourTestsData.DummyCommandWithResponse, BehaviourTestsData.DummyResponse>(
            _innerHandlerMock.Object,
            [_validatorMock.Object]);
    }

    [Fact]
    public async Task Handle_WhenNoValidators_ShouldCallInnerHandler()
    {
        var handlerWithoutValidators = new CommandHandler<BehaviourTestsData.DummyCommandWithResponse, BehaviourTestsData.DummyResponse>(
            _innerHandlerMock.Object,
            []);
        var command = new BehaviourTestsData.DummyCommandWithResponse();
        var response = new BehaviourTestsData.DummyResponse("Success");
        var expectedResult = Result<BehaviourTestsData.DummyResponse>.Success(response);

        _innerHandlerMock
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await handlerWithoutValidators.Handle(command);

        Assert.Equal(expectedResult, result);
        _innerHandlerMock.Verify(x => x.Handle(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldCallInnerHandler()
    {
        var command = new BehaviourTestsData.DummyCommandWithResponse();
        var response = new BehaviourTestsData.DummyResponse("Success");
        var expectedResult = Result<BehaviourTestsData.DummyResponse>.Success(response);
        var validationResult = new ValidationResult();

        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _innerHandlerMock
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command);

        Assert.Equal(expectedResult, result);
        _innerHandlerMock.Verify(x => x.Handle(command, It.IsAny<CancellationToken>()), Times.Once);
        _validatorMock.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnValidationError()
    {
        var command = new BehaviourTestsData.DummyCommandWithResponse();
        var validationFailure = new ValidationFailure("PropertyName", "Error message")
        {
            ErrorCode = "VALIDATION_ERROR"
        };
        var validationResult = new ValidationResult([validationFailure]);

        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var result = await _handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.IsType<ValidationError>(result.Error);

        var validationError = (ValidationError)result.Error;
        Assert.Single(validationError.Errors);
        Assert.Equal("VALIDATION_ERROR", validationError.Errors[0].Code);
        Assert.Equal("Error message", validationError.Errors[0].Message);
        Assert.Equal(ErrorType.Validation, validationError.Errors[0].ErrorType);

        _innerHandlerMock.Verify(x => x.Handle(It.IsAny<BehaviourTestsData.DummyCommandWithResponse>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenMultipleValidationFailures_ShouldReturnAllErrors()
    {
        var command = new BehaviourTestsData.DummyCommandWithResponse();
        var validationFailures = new[]
        {
            new ValidationFailure("Property1", "Error 1") { ErrorCode = "ERROR_1" },
            new ValidationFailure("Property2", "Error 2") { ErrorCode = "ERROR_2" }
        };
        var validationResult = new ValidationResult(validationFailures);

        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var result = await _handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.IsType<ValidationError>(result.Error);

        var validationError = (ValidationError)result.Error;
        Assert.Equal(2, validationError.Errors.Length);
        Assert.Contains(validationError.Errors, e => e.Code == "ERROR_1" && e.Message == "Error 1");
        Assert.Contains(validationError.Errors, e => e.Code == "ERROR_2" && e.Message == "Error 2");

        _innerHandlerMock.Verify(x => x.Handle(It.IsAny<BehaviourTestsData.DummyCommandWithResponse>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenMultipleValidators_ShouldRunAllValidators()
    {
        var secondValidatorMock = new Mock<IValidator<BehaviourTestsData.DummyCommandWithResponse>>();
        var handlerWithMultipleValidators = new CommandHandler<BehaviourTestsData.DummyCommandWithResponse, BehaviourTestsData.DummyResponse>(
            _innerHandlerMock.Object,
            [_validatorMock.Object, secondValidatorMock.Object]);

        var command = new BehaviourTestsData.DummyCommandWithResponse();
        var response = new BehaviourTestsData.DummyResponse("Success");
        var expectedResult = Result<BehaviourTestsData.DummyResponse>.Success(response);

        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        secondValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _innerHandlerMock
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await handlerWithMultipleValidators.Handle(command);

        Assert.Equal(expectedResult, result);
        _validatorMock.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()), Times.Once);
        secondValidatorMock.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()), Times.Once);
        _innerHandlerMock.Verify(x => x.Handle(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSomeValidatorsFailAndSomePass_ShouldReturnOnlyFailures()
    {
        var secondValidatorMock = new Mock<IValidator<BehaviourTestsData.DummyCommandWithResponse>>();
        var handlerWithMultipleValidators = new CommandHandler<BehaviourTestsData.DummyCommandWithResponse, BehaviourTestsData.DummyResponse>(
            _innerHandlerMock.Object,
            [_validatorMock.Object, secondValidatorMock.Object]);

        var command = new BehaviourTestsData.DummyCommandWithResponse();
        var validationFailure = new ValidationFailure("Property", "Error") { ErrorCode = "ERROR_CODE" };

        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([validationFailure]));

        secondValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var result = await handlerWithMultipleValidators.Handle(command);

        Assert.True(result.IsFailure);
        Assert.IsType<ValidationError>(result.Error);

        var validationError = (ValidationError)result.Error;
        Assert.Single(validationError.Errors);
        Assert.Equal("ERROR_CODE", validationError.Errors[0].Code);

        _innerHandlerMock.Verify(x => x.Handle(It.IsAny<BehaviourTestsData.DummyCommandWithResponse>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationTokenToInnerHandler()
    {
        var command = new BehaviourTestsData.DummyCommandWithResponse();
        var cancellationToken = new CancellationToken(true);
        var response = new BehaviourTestsData.DummyResponse("Success");
        var expectedResult = Result<BehaviourTestsData.DummyResponse>.Success(response);

        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), cancellationToken))
            .ReturnsAsync(new ValidationResult());

        _innerHandlerMock
            .Setup(x => x.Handle(command, cancellationToken))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, cancellationToken);

        Assert.Equal(expectedResult, result);
        _innerHandlerMock.Verify(x => x.Handle(command, cancellationToken), Times.Once);
        _validatorMock.Verify(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenInnerHandlerReturnsFailure_ShouldReturnSameFailure()
    {
        var command = new BehaviourTestsData.DummyCommandWithResponse();
        var expectedError = Error.Problem("INNER_ERROR", "Inner handler failed");
        var expectedResult = Result<BehaviourTestsData.DummyResponse>.Failure(expectedError);

        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _innerHandlerMock
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command);

        Assert.Equal(expectedResult, result);
        Assert.Equal(expectedError, result.Error);
    }

    [Fact]
    public async Task Handle_WhenInnerHandlerReturnsSuccess_ShouldReturnSameSuccessWithValue()
    {
        var command = new BehaviourTestsData.DummyCommandWithResponse();
        var response = new BehaviourTestsData.DummyResponse("Test Response");
        var expectedResult = Result<BehaviourTestsData.DummyResponse>.Success(response);

        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<BehaviourTestsData.DummyCommandWithResponse>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _innerHandlerMock
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command);

        Assert.Equal(expectedResult, result);
        Assert.True(result.IsSuccess);
        Assert.Equal(response, result.Value);
    }
}