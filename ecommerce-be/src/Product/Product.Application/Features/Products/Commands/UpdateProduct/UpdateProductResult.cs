namespace Product.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductResult(Guid Id, string Sku, string Name, decimal Price);