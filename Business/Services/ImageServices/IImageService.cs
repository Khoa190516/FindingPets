namespace FindingPets.Business.Services.ImageServices
{
    public interface IImageService
    {
        public Task<List<string>> ConvertImagesToBase64(List<IFormFile> images);
    }
}
