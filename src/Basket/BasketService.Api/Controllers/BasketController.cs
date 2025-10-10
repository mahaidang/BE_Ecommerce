// BasketService.Api/Controllers/BasketController.cs
using BasketService.Api.Models;
using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repo;
    private readonly TimeSpan _ttl;

    public BasketController(IBasketRepository repo, IConfiguration cfg)
    {
        _repo = repo;
        var minutes = cfg.GetValue<int?>("Redis:DefaultTtlMinutes") ?? 60;
        _ttl = TimeSpan.FromMinutes(minutes);
    }

    // Xem giỏ
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid userId)
        => Ok(await _repo.GetAsync(userId) ?? new Basket { UserId = userId, Items = new() });

    // Thêm/cập nhật item (upsert)
    [HttpPost("{userId:guid}/items")]
    public async Task<IActionResult> UpsertItem([FromRoute] Guid userId, [FromBody] UpsertItemDto dto)
    {
        if (dto.Quantity <= 0) return BadRequest("Quantity must be > 0");
        var item = new BasketItem
        {
            ProductId = dto.ProductId,
            Sku = dto.Sku.Trim(),
            Name = dto.Name.Trim(),
            UnitPrice = dto.UnitPrice,
            Quantity = dto.Quantity,
            Currency = dto.Currency.Trim().ToUpperInvariant()
        };
        await _repo.AddOrUpdateItemAsync(userId, item, _ttl);
        return Ok(await _repo.GetAsync(userId));
    }

    // Chỉ cập nhật số lượng
    [HttpPatch("{userId:guid}/items/{productId:guid}")]
    public async Task<IActionResult> UpdateQty([FromRoute] Guid userId, [FromRoute] Guid productId, [FromBody] UpdateQtyDto dto)
    {
        if (dto.Quantity <= 0) return BadRequest("Quantity must be > 0");

        var basket = await _repo.GetAsync(userId) ?? new Basket { UserId = userId };
        var it = basket.Items.FirstOrDefault(x => x.ProductId == productId);
        if (it is null) return NotFound();

        it.Quantity = dto.Quantity;
        await _repo.UpsertAsync(basket, _ttl);
        return Ok(basket);
    }

    // Xoá item
    [HttpDelete("{userId:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem([FromRoute] Guid userId, [FromRoute] Guid productId)
        => (await _repo.RemoveItemAsync(userId, productId, _ttl)) ? NoContent() : NotFound();

    // Xoá cả giỏ
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> Clear([FromRoute] Guid userId)
    {
        await _repo.ClearAsync(userId);
        return NoContent();
    }

    // GET /api/Basket/{userId}/enrich
    [HttpGet("{userId:guid}/enrich")]
    public async Task<IActionResult> GetEnriched([FromRoute] Guid userId, [FromServices] IHttpClientFactory http)
    {
        var basket = await _repo.GetAsync(userId) ?? new Basket { UserId = userId };
        if (basket.Items.Count == 0) return Ok(basket);

        var client = http.CreateClient("ProductApi");

        // gọi song song lấy detail theo ProductId
        var tasks = basket.Items.Select(async it =>
        {
            try
            {
                var p = await client.GetFromJsonAsync<ProductReadDto>($"/api/Products/{it.ProductId}");
                if (p is not null)
                {
                    it.Sku = string.IsNullOrWhiteSpace(p.sku) ? it.Sku : p.sku;
                    it.Name = string.IsNullOrWhiteSpace(p.name) ? it.Name : p.name;
                    // sync giá tham chiếu (tuỳ: có thể khóa khi checkout)
                    it.UnitPrice = p.price;
                    it.Currency = p.currency.ToUpperInvariant();
                }
            }
            catch { /* ignore not found/timeouts */ }
        });

        await Task.WhenAll(tasks);
        await _repo.UpsertAsync(basket); // lưu lại bản enrich (tùy)
        return Ok(basket);
    }

}
