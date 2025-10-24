using Microsoft.AspNetCore.Http;

namespace Product.Application.Abstractions.External;

public interface ICloudinaryService
{
    Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file, CancellationToken ct);
    Task<bool> DeleteImageAsync(string publicId, CancellationToken ct);
}
