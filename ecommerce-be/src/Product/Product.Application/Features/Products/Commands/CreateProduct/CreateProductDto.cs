namespace Product.Application.Features.Products.Commands.CreateProduct;

public sealed record CreateProductDto
(
    string Sku,
    string Name,
    string Slug,
    Guid? CategoryId,
    decimal Price,
    string Description,
    List<string> Variants,
    string Currency = "VND",    
    bool IsActive = true
);
