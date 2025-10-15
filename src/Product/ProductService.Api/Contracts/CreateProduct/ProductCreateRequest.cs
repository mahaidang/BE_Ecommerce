namespace ProductService.Api.Contracts.CreateProduct;

public record ProductCreateRequest(string Sku, string Name, string Slug, Guid? CategoryId, decimal Price, string Currency = "VND", bool IsActive = true);
