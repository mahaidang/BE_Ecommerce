namespace Product.Api.Contracts.CreateProduct;

public record ProductCreateRequest(
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
