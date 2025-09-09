namespace Domain.ReadModels.Categories;

public record CategoryReadModel
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; }
}
