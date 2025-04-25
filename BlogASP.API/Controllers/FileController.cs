using BlogASP.API.Infrastructure.Cloundinary;
using Microsoft.AspNetCore.Mvc;

namespace BlogASP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        public FileController(ICloudinaryService cloudinaryService) {
            _cloudinaryService = cloudinaryService;
        }
        // PUT: api/Post/{id}
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UpdatePostThumbail(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }

            try
            {
                // Upload imagen to cloundinary
                var uploadResult = await _cloudinaryService.UploadImageFromFileAsync(file);

                if (uploadResult?.SecureUrl == null)
                {
                    return StatusCode(500, "Failed to upload image: No secure URL returned.");
                }

                return Ok(new {ImageURL = uploadResult.SecureUrl.ToString() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
