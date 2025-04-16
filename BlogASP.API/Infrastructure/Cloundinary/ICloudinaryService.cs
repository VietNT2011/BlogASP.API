using CloudinaryDotNet.Actions;

namespace BlogASP.API.Infrastructure.Cloundinary
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageFromUrlAsync(string imageUrl);
        Task<ImageUploadResult> UploadImageFromFileAsync(IFormFile file);
    }
}
