using ProductService.Domain.Entities;

namespace ProductService.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct);
    Task<IEnumerable<Product>> SearchAsync(string? keyword, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    Task UpdateAsync(Product product, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);

    Task<(IEnumerable<Product> items, long total)> QueryAsync(
        string? keyword, Guid? categoryId, decimal? minPrice, decimal? maxPrice,
        int page, int pageSize, CancellationToken ct);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct);

}
