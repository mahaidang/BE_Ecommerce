namespace Product.Application.Features.Products.Queries;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, long Total);
