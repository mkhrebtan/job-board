namespace Domain.Abstraction.Interfaces;

public interface IUniqueCategoryNameSpecification
{
    bool IsSatisfiedBy(string name);
}
