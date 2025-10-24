namespace Product.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductDto
(
    Guid Id,
    string Sku,
    string Name,
    string Slug,
    Guid? CategoryId,
    decimal Price,
    string Currency = "VND",    
    bool IsActive = true
);
