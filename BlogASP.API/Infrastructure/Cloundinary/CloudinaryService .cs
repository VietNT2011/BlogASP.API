using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using dotenv.net;

namespace BlogASP.API.Infrastructure.Cloundinary
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            //DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            //var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL")
            //                    ?? throw new Exception("Cloudinary URL not configured");
            var cloudinaryUrl = "cloudinary://391985658538517:bSr2mUrDm7T6lGtACNi9nRYtYvA@dzbsqyibg";

            _cloudinary = new Cloudinary(cloudinaryUrl) { Api = { Secure = true } };
        }

        public async Task<ImageUploadResult> UploadImageFromFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };

                return await _cloudinary.UploadAsync(uploadParams);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while uploading file to Cloudinary.", ex);
            }
        }

        public async Task<ImageUploadResult> UploadImageFromUrlAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                throw new ArgumentException("Invalid URL");
            }

            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(imageUrl),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };

                return await _cloudinary.UploadAsync(uploadParams);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error occurred while uploading image from URL to Cloudinary.", ex);
            }
        }

    }
}
