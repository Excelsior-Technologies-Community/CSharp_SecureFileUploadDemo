

namespace SecureFileUploadDemo.Services
{
   
    /// Dummy antivirus – in real life you’d call an external AV engine.
    /// For demo:
    /// - If file name contains "virus" (case-insensitive) => mark as infected.
    /// - Else => safe.
    /// </summary>
    public class DummyAntivirusScanner : IAntivirusScanner
    {
        public Task<bool> ScanAsync(IFormFile file)
        {
            var name = file.FileName.ToLowerInvariant();
            bool isSafe = !name.Contains("virus");
            return Task.FromResult(isSafe);
        }
    }
}
