using Refit;


namespace Basket.Application.Abstractions.External;

public interface IProductApi
{
    // Lấy danh sách sản phẩm theo IDs
    [Get("/api/products/batch")]
    Task<List<ProductDto>> GetProductsByIdsAsync(
        [Query(CollectionFormat = CollectionFormat.Csv)] IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);
}

// Dto riêng cho BasketService (tách khỏi Product domain)
public record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    string ImageUrl
);
