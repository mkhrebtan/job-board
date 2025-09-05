using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Repos.Categories;
using Domain.Repos.Vacancies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Vacancies.UpdateCategory;

internal sealed class UpdateVacancyCategoryCommandHandler : ICommandHandler<UpdateVacancyCategoryCommand>
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVacancyCategoryCommandHandler(IVacancyRepository vacancyRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _vacancyRepository = vacancyRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateVacancyCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(command.Id, cancellationToken);
        if (vacancy is null)
        {
            return Result.Failure(Error.NotFound("Vacancy.NotFound", $"Vacancy with ID {command.Id} was not found."));
        }

        var category = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
        if (category is null)
        {
            return Result.Failure(Error.NotFound("Category.NotFound", "The category was not found."));
        }

        var categoryId = new CategoryId(command.CategoryId);
        var result = vacancy.UpdateCategoryId(categoryId);
        if (result.IsFailure)
        {
            return result;
        }

        _vacancyRepository.Update(vacancy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
