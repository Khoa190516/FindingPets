using Microsoft.EntityFrameworkCore.Metadata;

namespace FindingPets.Business.Services.ImageServices
{
    public class ImageService : IImageService
    {
        public async Task<List<string>> ConvertImagesToBase64(List<IFormFile> images)
        {
            List<string> imagesBase64 = new List<string>();

            try
            {
                await using var memoryStream = new MemoryStream();
                
                foreach (var image in images)
                {
                    await image.CopyToAsync(memoryStream);
                    var bytes = memoryStream.ToArray();
                    var imageBase64 = Convert.ToBase64String(bytes);
                    imageBase64 = "data:image/gif;base64," + imageBase64;

                    imagesBase64.Add(imageBase64);
                }
                return imagesBase64;
            }
            catch (Exception ex)
            {
                throw new Exception("Convert image to base64 failed with error: " + ex.Message);
            }
        }
    }
}
