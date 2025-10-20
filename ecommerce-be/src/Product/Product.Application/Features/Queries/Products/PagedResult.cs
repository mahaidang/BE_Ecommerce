namespace ProductService.Application.Features.Queries.Products;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, long Total);
