using Mapster;
using MediatR;
using Product.Application.Abstractions.Persistence;
using Product.Application.Features.Products.Dtos;
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
        {
            var mainImg = p.Images?
                .FirstOrDefault(i => i.IsMain)
                ?? p.Images?.FirstOrDefault();

            // 🧩 Tạo record mới ProductDto, truyền mainImg vào tham số cuối
            return new ProductDto(
                Id: p.Id,
                Sku: p.Sku,
                Name: p.Name,
                Slug: p.Slug,
                CategoryId: p.CategoryId,
                Price: p.Price,
                Currency: p.Currency,
                IsActive: p.IsActive,
                CreatedAtUtc: p.CreatedAtUtc,
                UpdatedAtUtc: p.UpdatedAtUtc,
                MainImage: mainImg is null
                    ? null
                    : new ProductImageDto
                    {
                        Url = mainImg.Url,
                        PublicId = mainImg.PublicId,
                        IsMain = mainImg.IsMain,
                        Alt = mainImg.Alt
                    }
            );
        }).ToList();

        return new PagedResult<ProductDto>(dtos, q.Page, q.PageSize, total);
    }
}