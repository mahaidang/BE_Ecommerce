namespace Product.Application.Features.Categories.Commands.CreateCategory;

public record CategoryDto
(
    string Name,
    string Slug,
    string Description = ""
);