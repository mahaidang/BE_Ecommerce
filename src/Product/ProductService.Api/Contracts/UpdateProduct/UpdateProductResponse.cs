namespace ProductService.Api.Contracts.UpdateProduct;

// Api/Contracts/Products/UpdateProductResponse.cs
public sealed record UpdateProductResponse(
    Guid Id,
    string Sku,
    string Name,
    string Slug,
    Guid? CategoryId,
    decimal Price,
    string Currency,
    bool IsActive,
    DateTime UpdatedAtUtc
);