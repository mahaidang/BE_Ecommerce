using Product.Domain.Entities;

namespace Product.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<ProductModel?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<ProductModel>> GetAllAsync(CancellationToken ct);
    Task<IEnumerable<ProductModel>> SearchAsync(string? keyword, CancellationToken ct);
    Task AddAsync(ProductModel product, CancellationToken ct);
    Task UpdateAsync(ProductModel product, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);

    Task<(IEnumerable<ProductModel> items, long total)> QueryAsync(
        string? keyword, Guid? categoryId, decimal? minPrice, decimal? maxPrice,
        int page, int pageSize, CancellationToken ct);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct);

    Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(Guid productId, CancellationToken ct);

    Task UpdateImagesAsync(Guid id, ProductImage img, CancellationToken ct);

}
