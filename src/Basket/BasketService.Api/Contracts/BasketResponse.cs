namespace BasketService.Api.Contracts;

public sealed record BasketResponse(Guid UserId, List<BasketItemResponse> Items);
public sealed record BasketItemResponse(Guid ProductId, string Sku, string Name,
                                        decimal UnitPrice, int Quantity, string Currency);
