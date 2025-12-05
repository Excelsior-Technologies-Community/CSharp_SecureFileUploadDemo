using Microsoft.EntityFrameworkCore;
using SecureFileUploadDemo.Data;
using SecureFileUploadDemo.Models;

namespace SecureFileUploadDemo.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly AppDbContext _db;
        private readonly ICloudStorageService _cloud;
        private readonly IAntivirusScanner _antivirus;
        private readonly IConfiguration _config;

        private readonly long _maxBytes;
        private readonly HashSet<string> _allowedMime;
        private readonly HashSet<string> _allowedExt;

        public FileUploadService(
            AppDbContext db,
            ICloudStorageService cloud,
            IAntivirusScanner antivirus,
            IConfiguration config)
        {
            _db = db;
            _cloud = cloud;
            _antivirus = antivirus;
            _config = config;

            _maxBytes = (long)(config.GetSection("FileUpload")["MaxSizeMB"] is string mbStr &&
                               long.TryParse(mbStr, out var mb)
                ? mb
                : 500) * 1024L * 1024L;

            _allowedMime = config.GetSection("FileUpload:AllowedMimeTypes").Get<string[]>()?
                               .Select(m => m.ToLowerInvariant())
                               .ToHashSet()
                           ?? new HashSet<string>();

            _allowedExt = config.GetSection("FileUpload:AllowedExtensions").Get<string[]>()?
                              .Select(e => e.ToLowerInvariant())
                              .ToHashSet()
                          ?? new HashSet<string>();
        }

        public async Task<FileRecord?> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("No file selected.");

            if (file.Length > _maxBytes)
                throw new InvalidOperationException("File is too large.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var mime = (file.ContentType ?? "").ToLowerInvariant();

            if (_allowedExt.Any() && !_allowedExt.Contains(ext))
                throw new InvalidOperationException("File type not allowed (extension).");

            if (_allowedMime.Any() && !_allowedMime.Contains(mime))
                throw new InvalidOperationException("File type not allowed (MIME).");

            // Dummy AV scan
            var isSafe = await _antivirus.ScanAsync(file);
            if (!isSafe)
                throw new InvalidOperationException("Virus detected in file (dummy AV).");

            var id = Guid.NewGuid();
            var storedName = $"{id}{ext}";
            var signedToken = Guid.NewGuid().ToString("N");

            await _cloud.UploadAsync(file, storedName);

            var record = new FileRecord
            {
                Id = id,
                OriginalFileName = file.FileName,
                StoredFileName = storedName,
                ContentType = mime,
                SizeBytes = file.Length,
                IsSafe = true,
                SignedToken = signedToken,
                UploadedAt = DateTimeOffset.Now
            };

            _db.FileRecords.Add(record);
            await _db.SaveChangesAsync();

            return record;
        }

        public async Task<IEnumerable<FileRecord>> GetAllAsync()
        {
            return (await _db.FileRecords.ToListAsync())
                .OrderByDescending(f => f.UploadedAt);
        }



        public async Task<FileRecord?> GetByIdAsync(Guid id)
        {
            return await _db.FileRecords.FindAsync(id);
        }
    }
}
