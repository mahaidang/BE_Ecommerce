using BasketService.Application.Abstractions.External;
using BasketService.Application.Abstractions.Persistence;
using BasketService.Domain.Entities;
using MediatR;

namespace BasketService.Application.Features.Baskets.Queries;

public sealed class GetEnrichedBasketHandler(IBasketRepository repo, IProductCatalogClient productClient)
    : IRequestHandler<GetEnrichedBasketQuery, Basket>
{
    public async Task<Basket> Handle(GetEnrichedBasketQuery q, CancellationToken ct)
    {
        var basket = await repo.GetAsync(q.UserId, ct) ?? new Basket { UserId = q.UserId, Items = new() };
        if (basket.Items.Count == 0) return basket;

        // có thể giới hạn song song: SemaphoreSlim(8)
        var tasks = basket.Items.Select(async it =>
        {
            try
            {
                var p = await productClient.GetByIdAsync(it.ProductId, ct);
                if (p is not null)
                {
                    it.Sku = string.IsNullOrWhiteSpace(p.sku) ? it.Sku : p.sku;
                    it.Name = string.IsNullOrWhiteSpace(p.name) ? it.Name : p.name;
                    it.UnitPrice = p.price;
                    it.Currency = p.currency.ToUpperInvariant();
                }
            }
            catch { /* log warning nếu cần */ }
        });

        await Task.WhenAll(tasks);
        await repo.UpsertAsync(basket, null, ct); // tuỳ, muốn lưu lại bản enrich hay không
        return basket;
    }
}