using Basket.Application.Abstractions.External;
using BasketService.Application.Abstractions.Persistence;
using MediatR;
using Mapster;


namespace BasketService.Application.Features.Baskets.Queries;


public class GetBasketHandler : IRequestHandler<GetBasketQuery, BasketDto>
{
    private readonly IBasketRepository _repo;
    private readonly IProductApi _productApi;

    public GetBasketHandler(IBasketRepository repo, IProductApi productApi)
    {
        _repo = repo;
        _productApi = productApi;
    }

    public async Task<BasketDto> Handle(GetBasketQuery request, CancellationToken ct)
    {
        var basket = await _repo.GetAsync(request.UserId, ct);
        if (basket == null) return new BasketDto(request.UserId, new());

        var ids = basket.Items.Select(i => i.ProductId);
        var products = await _productApi.GetProductsByIdsAsync(ids, ct);

        var items = products.Adapt<List<BasketItemDto>>();

        return new BasketDto(basket.UserId, items);
    }
}

public record BasketItemDto(
    Guid ImageUrl,
    string Name,
    decimal Price
);

public record BasketDto(
    Guid UserId,
    List<BasketItemDto> Items
);