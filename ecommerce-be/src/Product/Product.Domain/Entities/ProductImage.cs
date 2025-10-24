using MongoDB.Bson.Serialization.Attributes;

namespace Product.Domain.Entities;

public class ProductImage
{
    [BsonElement("url")]
    public string Url { get; set; } = string.Empty;

    [BsonElement("publicId")]
    public string PublicId { get; set; } = string.Empty;

    [BsonElement("isMain")]
    public bool IsMain { get; set; }

    [BsonElement("alt")]
    public string? Alt { get; set; }
}
