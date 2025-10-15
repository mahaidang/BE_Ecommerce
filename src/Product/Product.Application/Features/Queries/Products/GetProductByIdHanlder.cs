using MediatR;
using ProductService.Application.Abstractions.Persistence;
using Mapster;

namespace ProductService.Application.Features.Queries.Products;

public class GetProductByIdHanlder : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _prods;

    public GetProductByIdHanlder(IProductRepository prods)
    {
        _prods = prods;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery req, CancellationToken ct)
    {
        var id = req.Id;
        var product = await _prods.GetByIdAsync(id, ct);
        if (product is null)
            throw new InvalidOperationException($"Product with id '{id}' not found.");
        return product.Adapt<ProductDto>();
    }
}
