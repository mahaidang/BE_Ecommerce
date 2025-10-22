namespace Product.Api.Contracts.CreateProduct;
public record ProductCreateResponse(string Sku, string Name, string Slug, Guid? CategoryId, decimal Price, string Currency = "VND", bool IsActive = true);
