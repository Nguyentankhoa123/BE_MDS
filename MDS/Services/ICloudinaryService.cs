namespace MDS.Services
{
    public interface ICloudinaryService
    {
        Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folder);
    }
}
