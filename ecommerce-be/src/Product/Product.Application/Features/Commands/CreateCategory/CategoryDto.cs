namespace Product.Application.Features.Commands.CreateCategory;

public record CategoryDto
(
    string Name,
    string Slug,
    string Description = ""
);