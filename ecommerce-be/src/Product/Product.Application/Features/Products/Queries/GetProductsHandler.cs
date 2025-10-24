using Mapster;
using MediatR;
using Product.Application.Abstractions.Persistence;
using System.Linq.Dynamic.Core;


namespace Product.Application.Features.Products.Queries;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _prods;

    public GetProductsHandler(IProductRepository prods)
    {
        _prods = prods;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery q, CancellationToken ct)
    {
        var (items, total) = await _prods.QueryAsync(
            q.Keyword, q.CategoryId, q.MinPrice, q.MaxPrice,
            q.Page, q.PageSize, ct);

        var dtos = items.Select(p =>
            p.Adapt<ProductDto>()
        ).ToList();

        return new PagedResult<ProductDto>(dtos, q.Page, q.PageSize, total);
    }
}