using Mapster;
using MediatR;
using Product.Application.Abstractions.Persistence;
using Product.Application.Features.Products.Dtos;

namespace Product.Application.Features.Products.Queries;

public sealed record GetFull(Guid Id) : IRequest<ProductFullDto>;

public class GetFullHanlder : IRequestHandler<GetFull, ProductFullDto>
{
    private readonly IProductRepository _prods;

    public GetFullHanlder(IProductRepository prods)
    {
        _prods = prods;
    }

    public async Task<ProductFullDto> Handle(GetFull req, CancellationToken ct)
    {
        var id = req.Id;
        var product = await _prods.GetByIdAsync(id, ct);
        if (product is null)
            throw new InvalidOperationException($"Product with id '{id}' not found.");
        return product.Adapt<ProductFullDto>();
    }
}