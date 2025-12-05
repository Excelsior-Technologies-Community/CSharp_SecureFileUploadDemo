using SecureFileUploadDemo.Models;

namespace SecureFileUploadDemo.Services
{
    public interface IFileUploadService
    {
        Task<FileRecord?> UploadAsync(IFormFile file);
        Task<IEnumerable<FileRecord>> GetAllAsync();
        Task<FileRecord?> GetByIdAsync(Guid id);
    }
}
