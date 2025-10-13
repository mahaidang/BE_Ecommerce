namespace ProductService.Api.Contracts.UpdateProduct;

// Api/Contracts/Products/UpdateProductRequest.cs
public sealed record UpdateProductRequest(
    string Sku,
    string Name,
    string Slug,
    Guid? CategoryId,
    decimal Price,
    string Currency,
    bool IsActive
);


