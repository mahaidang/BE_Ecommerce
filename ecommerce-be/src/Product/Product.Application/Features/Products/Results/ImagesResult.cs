namespace Product.Application.Features.Products.Results;

public record ImageResult(string PublicId, string Url, string Alt);

public record ImagesResult<T>(IReadOnlyList<T> Images);
