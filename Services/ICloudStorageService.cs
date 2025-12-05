

namespace SecureFileUploadDemo.Services
{
    public interface ICloudStorageService
    {
        Task<string> UploadAsync(IFormFile file, string blobName, CancellationToken cancellationToken = default);
        Task<Stream> DownloadAsync(string blobName, CancellationToken cancellationToken = default);
    }
}
