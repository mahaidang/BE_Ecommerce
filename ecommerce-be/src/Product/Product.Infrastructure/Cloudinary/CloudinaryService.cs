using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Product.Application.Abstractions.External;

namespace Product.Infrastructure.Cloudinary;

public class CloudinaryService : ICloudinaryService
{
    private readonly CloudinaryDotNet.Cloudinary _cloud;

    public CloudinaryService(CloudinarySettings settings)
    {
        var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
        _cloud = new CloudinaryDotNet.Cloudinary(account);
    }

    public async Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "ecommerce/products"
        };

        var result = await _cloud.UploadAsync(uploadParams);
        if (result.StatusCode != System.Net.HttpStatusCode.OK)
            throw new Exception($"Upload failed: {result.Error?.Message}");

        return (result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task<bool> DeleteImageAsync(string publicId, CancellationToken ct)
    {
        var result = await _cloud.DestroyAsync(new DeletionParams(publicId));
        return result.Result == "ok";
    }
}