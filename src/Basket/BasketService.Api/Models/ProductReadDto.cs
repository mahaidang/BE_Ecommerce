// BasketService.Api/Models/ProductReadDto.cs
namespace BasketService.Api.Models;
public sealed class ProductReadDto
{
    public Guid id { get; set; }
    public string sku { get; set; } = "";
    public string name { get; set; } = "";
    public string slug { get; set; } = "";
    public decimal price { get; set; }
    public string currency { get; set; } = "VND";
}
