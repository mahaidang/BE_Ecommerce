using Product.Application.Features.Products.Dtos;

namespace Product.Application.Features.Products.Queries;

public record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    string Slug,
    Guid? CategoryId,
    decimal Price,
    string Currency,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    ProductImageDto? MainImage
);