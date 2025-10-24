namespace Product.Application.Features.Categories.Commands.CreateCategory;

public sealed record CategoryResult(Guid Id, string Name, string Slug, string Description = ""
);