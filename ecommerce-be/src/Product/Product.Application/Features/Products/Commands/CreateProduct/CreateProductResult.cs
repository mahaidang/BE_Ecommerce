namespace Product.Application.Features.Products.Commands.CreateProduct;

public sealed record CreateProductResult(Guid Id, string Sku, string Name, decimal Price);