namespace ProductService.Application.Features.Commands.UpdateProduct;

public sealed record CreateProductResult(Guid Id, string Sku, string Name, decimal Price);