namespace SecureFileUploadDemo.Services
{
    public interface IAntivirusScanner
    {
        Task<bool> ScanAsync(IFormFile file);
    }
}
