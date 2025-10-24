using MediatR;
using Product.Application.Abstractions.External;
using Product.Application.Abstractions.Persistence;

namespace ProductService.Application.Products.Commands;

public record DeleteProductImageCommand(Guid ProductId, string PublicId) : IRequest<bool>;

public class DeleteProductImageHandler : IRequestHandler<DeleteProductImageCommand, bool>
{
    private readonly IProductRepository _products;
    private readonly ICloudinaryService _cloud;

    public DeleteProductImageHandler(IProductRepository products, ICloudinaryService cloud)
    {
        _products = products;
        _cloud = cloud;
    }

    public async Task<bool> Handle(DeleteProductImageCommand req, CancellationToken ct)
    {
        var product = await _products.GetByIdAsync(req.ProductId, ct);
        if (product is null) return false;

        var image = product.Images.FirstOrDefault(i => i.PublicId == req.PublicId);
        if (image is null) return false;

        await _cloud.DeleteImageAsync(req.PublicId, ct);

        product.Images.Remove(image);
        await _products.UpdateAsync(product, ct);
        return true;
    }
}
