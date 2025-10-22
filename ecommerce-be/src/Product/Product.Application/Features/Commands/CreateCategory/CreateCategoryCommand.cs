using MediatR;
using Product.Application.Abstractions.Persistence;
using Product.Application.Features.Commands.UpdateCategory;
using Product.Domain.Entities;

namespace Product.Application.Features.Commands.CreateCategory;

public record CreateCategoryCommand(CategoryDto Dto) : IRequest<CategoryResult>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResult>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<CategoryResult> Handle(CreateCategoryCommand req, CancellationToken cancellationToken)
    {
        var dto = req.Dto;
        var category = new Category
        {
            Name = dto.Name.Trim(),
            Slug = dto.Slug.Trim().ToLowerInvariant(),
            Description = dto.Description?.Trim()
        };

        try 
        {
            await _categoryRepository.AddAsync(category, cancellationToken);
        }
        catch (Exception ex)
        {
            // Handle exceptions such as duplicate key errors
            throw new InvalidOperationException("Category could not be created", ex);
        }
        var rs = new CategoryResult(category.Id, category.Name, category.Slug, category.Description);
        return rs;
    }
}