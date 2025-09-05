using Application.Commands.Categories.Delete;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Repos.Categories;
using Domain.Shared.ErrorHandling;
using Moq;

namespace Application.Tests.CommandHandlers.Categories;

public class DeleteCategoryCommandHandlerTests
{
    private readonly DeleteCategoryCommandHandler _handler;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public DeleteCategoryCommandHandlerTests()
    {
        _handler = new DeleteCategoryCommandHandler(_categoryRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingCategoryWithoutAssignedVacancies_ShouldReturnSuccess()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);
        var existingCategory = Category.Create("Test Category", "TEST CATEGORY");

        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(repo => repo.HasAssignedVacancies(existingCategory, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);
        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.HasAssignedVacancies(existingCategory, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.Remove(existingCategory), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ShouldReturnNotFoundError()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);

        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal("Category.NotFound", result.Error.Code);
        Assert.Equal("The category was not found.", result.Error.Message);
        Assert.Equal(ErrorType.NotFound, result.Error.ErrorType);

        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.HasAssignedVacancies(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
        _categoryRepositoryMock.Verify(repo => repo.Remove(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithCategoryHavingAssignedVacancies_ShouldReturnValidationError()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);
        var existingCategory = Category.Create("Test Category", "TEST CATEGORY");

        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(repo => repo.HasAssignedVacancies(existingCategory, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal("Category.HasAssignedVacancies", result.Error.Code);
        Assert.Equal("The category has assigned vacancies and cannot be deleted.", result.Error.Message);
        Assert.Equal(ErrorType.Validation, result.Error.ErrorType);

        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.HasAssignedVacancies(existingCategory, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.Remove(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectCancellationToken()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);
        var existingCategory = Category.Create("Test Category", "TEST CATEGORY");
        var cancellationToken = new CancellationToken(true);

        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(categoryId, cancellationToken))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(repo => repo.HasAssignedVacancies(existingCategory, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, cancellationToken), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.HasAssignedVacancies(existingCategory, cancellationToken), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("12345678-1234-1234-1234-123456789abc")]
    [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
    public async Task Handle_WithDifferentGuidFormats_ShouldHandleCorrectly(string guidString)
    {
        // Arrange
        var categoryId = Guid.Parse(guidString);
        var command = new DeleteCategoryCommand(categoryId);

        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Category.NotFound", result.Error.Code);
        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);
        var expectedException = new InvalidOperationException("Database connection failed");

        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command));
        Assert.Equal(expectedException.Message, exception.Message);

        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.Remove(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenHasAssignedVacanciesThrowsException_ShouldPropagateException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);
        var existingCategory = Category.Create("Test Category", "TEST CATEGORY");
        var expectedException = new InvalidOperationException("Vacancy check failed");

        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(repo => repo.HasAssignedVacancies(existingCategory, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command));
        Assert.Equal(expectedException.Message, exception.Message);

        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.HasAssignedVacancies(existingCategory, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.Remove(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUnitOfWorkSaveChangesThrowsException_ShouldPropagateException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);
        var existingCategory = Category.Create("Test Category", "TEST CATEGORY");
        var expectedException = new InvalidOperationException("Save changes failed");

        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(repo => repo.HasAssignedVacancies(existingCategory, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command));
        Assert.Equal(expectedException.Message, exception.Message);

        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.HasAssignedVacancies(existingCategory, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.Remove(existingCategory), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDefaultCancellationToken_ShouldWork()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);
        var existingCategory = Category.Create("Test Category", "TEST CATEGORY");

        _categoryRepositoryMock
            .Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        _categoryRepositoryMock
            .Setup(repo => repo.HasAssignedVacancies(existingCategory, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command); // Using default cancellation token

        // Assert
        Assert.True(result.IsSuccess);
        _categoryRepositoryMock.Verify(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.HasAssignedVacancies(existingCategory, It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.Remove(existingCategory), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}