using MediatR;
using Product.Application.Abstractions.Persistence;
using Product.Application.Features.Products.Results;

namespace Product.Application.Features.Products.Queries;

public record GetImagesByProdIdQuery(Guid ProductId) : IRequest<ImagesResult<ImageResult>>;

public class GetImagesByProdIdHandler : IRequestHandler<GetImagesByProdIdQuery, ImagesResult<ImageResult>>
{
    private readonly IProductRepository _products;
    public GetImagesByProdIdHandler(IProductRepository products)
    {
        _products = products;
    }
    public async Task<ImagesResult<ImageResult>> Handle(GetImagesByProdIdQuery req, CancellationToken ct)
    {
        var images = await _products.GetImagesByProductIdAsync(req.ProductId, ct);
        
        var imageResults = new ImagesResult<ImageResult>(
            images.Select(img => new ImageResult(img.PublicId, img.Url)).ToList()   
        );
        
        return imageResults;
    }
}   