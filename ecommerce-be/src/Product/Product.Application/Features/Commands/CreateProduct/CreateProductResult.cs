namespace Product.Application.Features.Commands.CreateProduct;

public sealed record CreateProductResult(Guid Id, string Sku, string Name, decimal Price);