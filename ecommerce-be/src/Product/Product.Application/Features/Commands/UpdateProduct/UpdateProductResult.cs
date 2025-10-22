namespace Product.Application.Features.Commands.UpdateProduct;

public sealed record UpdateProductResult(Guid Id, string Sku, string Name, decimal Price);