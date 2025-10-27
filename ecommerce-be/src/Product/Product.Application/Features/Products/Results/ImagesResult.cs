namespace Product.Application.Features.Products.Results;

public record ImageResult(string PublicId, string Url);

public record ImagesResult<T>(IReadOnlyList<T> Images);
