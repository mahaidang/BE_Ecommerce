namespace BasketService.Api.Contracts;

public sealed record UpsertItemRequest(Guid ProductId, string Sku, string Name,
                                      decimal UnitPrice, int Quantity, string Currency);