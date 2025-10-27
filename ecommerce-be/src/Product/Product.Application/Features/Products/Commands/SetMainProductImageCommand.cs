using MediatR;
using Product.Application.Abstractions.Persistence;

namespace Product.Application.Features.Products.Commands;

public record SetMainProductImageCommand(Guid ProductId, string ImageId) : IRequest<bool>;

public class SetMainProductImageHandler : IRequestHandler<SetMainProductImageCommand, bool>
{
    private readonly IProductRepository _products;
    public SetMainProductImageHandler(IProductRepository products)
    {
        _products = products;
    }
    public async Task<bool> Handle(SetMainProductImageCommand req, CancellationToken ct)
    {
        var product = await _products.ExistsAsync(req.ProductId, ct);
        if (!product) return false;
        var result = await _products.SetMainImageAsync(req.ProductId, req.ImageId, ct);
        return result;
    }
}