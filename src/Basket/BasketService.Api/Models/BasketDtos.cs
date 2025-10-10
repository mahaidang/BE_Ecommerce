// BasketService.Api/Models/BasketDtos.cs
namespace BasketService.Api.Models;

public record UpsertItemDto(Guid ProductId, string Sku, string Name, decimal UnitPrice, int Quantity, string Currency = "VND");
public record UpdateQtyDto(int Quantity);
