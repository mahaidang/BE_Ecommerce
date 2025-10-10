namespace ProductService.Api;

public record ProductCreateDto(string Sku, string Name, string Slug, Guid? CategoryId, decimal Price, string Currency = "VND", bool IsActive = true);
public record ProductUpdateDto(string Sku, string Name, string Slug, Guid? CategoryId, decimal Price, string Currency = "VND", bool IsActive = true);
