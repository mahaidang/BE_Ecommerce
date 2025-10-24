namespace Product.Application.Features.Products.Dtos;

public class ProductImageDto
{
    public string Url { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public string? Alt { get; set; }
}
