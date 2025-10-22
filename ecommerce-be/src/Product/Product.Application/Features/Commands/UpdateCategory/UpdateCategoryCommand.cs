using MediatR;
using Product.Application.Abstractions.Persistence;
using Product.Application.Features.Commands.CreateCategory;

namespace Product.Application.Features.Commands.UpdateCategory;

public record class UpdateCategoryCommand(UpdateCategoryDto Dto) : IRequest<UpdateCategoryResult>;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResult>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<UpdateCategoryResult> Handle(UpdateCategoryCommand command, CancellationToken ct)
    {
        var dto = command.Dto;
        var category = new Domain.Entities.Category
        {
            Id = dto.Id,
            Name = dto.Name.Trim(),
            Slug = dto.Slug.Trim().ToLowerInvariant(),
            Description = dto.Description.Trim()
        };
        try
        {
            await _categoryRepository.UpdateAsync(category, ct);
        }
        catch (MongoDB.Driver.MongoWriteException mwe) when (mwe.WriteError.Category == MongoDB.Driver.ServerErrorCategory.DuplicateKey)
        {
            throw new InvalidOperationException("Sku or Slug already exists");
        }
        return new UpdateCategoryResult(category.Id, category.Name, category.Slug, category.Description);
    }
}

public record UpdateCategoryResult(
    Guid Id,
    string Sku,
    string Name,
    string Slug,
    string Description = ""
);


public record UpdateCategoryDto(
    Guid Id,
    string Sku,
    string Name,
    string Slug,
    string Description = ""
);