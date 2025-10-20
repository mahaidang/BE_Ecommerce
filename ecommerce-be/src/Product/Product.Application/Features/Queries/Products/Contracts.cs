using MediatR;
using ProductService.Domain.Entities;
using System.Linq.Dynamic.Core;

namespace ProductService.Application.Features.Queries.Products;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
public sealed record GetProductsQuery(
    string? Keyword,
    Guid? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<ProductDto>>;