namespace BasketService.Domain.Entities;

public class Basket
{
    public Guid UserId { get; set; }                 // khoá chính giỏ = UserId
    public List<BasketItem> Items { get; set; } = new();
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
}

public class BasketItem
{
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string Currency { get; set; } = "VND";
}
