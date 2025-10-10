using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductService.Api;

// Document sản phẩm lưu trong Mongo
public class ProductDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]     // lưu Guid dạng string
    public Guid Id { get; set; }

    [BsonElement("sku")]
    public string Sku { get; set; } = "";

    [BsonElement("name")]
    public string Name { get; set; } = "";

    [BsonElement("slug")]
    public string Slug { get; set; } = "";

    [BsonElement("categoryId")]
    [BsonRepresentation(BsonType.String)]
    public Guid? CategoryId { get; set; }

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("currency")]
    public string Currency { get; set; } = "VND";

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdAtUtc")]
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAtUtc")]
    public DateTime? UpdatedAtUtc { get; set; }
}
