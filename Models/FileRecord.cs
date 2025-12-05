using System;

namespace SecureFileUploadDemo.Models
{
    public class FileRecord
    {
        public Guid Id { get; set; }                  // Primary key
        public string OriginalFileName { get; set; } = null!;
        public string StoredFileName { get; set; } = null!;  // Blob name
        public string ContentType { get; set; } = null!;
        public long SizeBytes { get; set; }
        public bool IsSafe { get; set; }
        public string SignedToken { get; set; } = null!;  // For signed URL
        public DateTimeOffset UploadedAt { get; set; }
    }
}
