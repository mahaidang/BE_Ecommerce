namespace BasketService.Domain.Entities;

public class Basket
{
    public Guid UserId { get; set; }                 // khoá chính giỏ = UserId
    public List<BasketItem> Items { get; set; } = new();
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

public class BasketItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
