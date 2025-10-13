using BasketService.Application.Abstractions.External;
using System.Net.Http.Json;

namespace BasketService.Infrastructure.External;

public sealed class ProductCatalogClient(HttpClient http) : IProductCatalogClient
{
    public Task<ProductReadDto?> GetByIdAsync(Guid productId, CancellationToken ct)
        => http.GetFromJsonAsync<ProductReadDto>($"/api/v1/products/{productId}", ct);
}
