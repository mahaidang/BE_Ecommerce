using MediatR;
using Microsoft.AspNetCore.Http;
using Product.Application.Abstractions.External;
using Product.Application.Abstractions.Persistence;
using Product.Domain.Entities;

namespace Product.Application.Features.Products.Commands;

public record UploadProductImageCommand(Guid ProductId, IFormFile File, bool IsMain)
    : IRequest<bool>;

public class UploadProductImageHandler : IRequestHandler<UploadProductImageCommand, bool>
{
    private readonly IProductRepository _products;
    private readonly ICloudinaryService _cloud;

    public UploadProductImageHandler(IProductRepository products, ICloudinaryService cloud)
    {
        _products = products;
        _cloud = cloud;
    }

    public async Task<bool> Handle(UploadProductImageCommand req, CancellationToken ct)
    {
        var product = await _products.ExistsAsync(req.ProductId, ct);
        if (!product) return false;

        var (url, publicId) = await _cloud.UploadImageAsync(req.File, ct);


        var img = new ProductImage
        {
            Url = url,
            PublicId = publicId,
            IsMain = req.IsMain,
            Alt = req.File.FileName
        };

        await _products.UpdateImagesAsync(req.ProductId, img, ct);
        return true;
    }
}

