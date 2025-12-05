using Azure.Storage.Blobs;


namespace SecureFileUploadDemo.Services
{
    public class AzureBlobStorageService : ICloudStorageService
    {
        private readonly BlobContainerClient _container;

        public AzureBlobStorageService(IConfiguration config)
        {
            var connString = config.GetSection("AzureStorage")["ConnectionString"];
            var containerName = config.GetSection("AzureStorage")["ContainerName"] ?? "uploads";

            var serviceClient = new BlobServiceClient(connString);
            _container = serviceClient.GetBlobContainerClient(containerName);

            // Ensure container exists
            _container.CreateIfNotExists();
        }

        public async Task<string> UploadAsync(IFormFile file, string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = _container.GetBlobClient(blobName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);

            return blobName;
        }

        public async Task<Stream> DownloadAsync(string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = _container.GetBlobClient(blobName);

            var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
            return response.Value.Content;
        }
    }
}
