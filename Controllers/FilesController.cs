using Microsoft.AspNetCore.Mvc;
using SecureFileUploadDemo.Services;


namespace SecureFileUploadDemo.Controllers
{
    public class FilesController : Controller
    {
        private readonly IFileUploadService _fileService;
        private readonly ICloudStorageService _cloud;

        public FilesController(
            IFileUploadService fileService,
            ICloudStorageService cloud)
        {
            _fileService = fileService;
            _cloud = cloud;
        }

        // GET: /Files
        public async Task<IActionResult> Index()
        {
            var files = await _fileService.GetAllAsync();
            return View(files);
        }

        // GET: /Files/Upload
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        // POST: /Files/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(600_000_000)] // 600MB
        public async Task<IActionResult> UploadFile()
        {
            var file = Request.Form.Files["file"];

            if (file == null)
            {
                ModelState.AddModelError("", "Please select a file.");
                return View("Upload");
            }

            try
            {
                var record = await _fileService.UploadAsync(file);
                TempData["Success"] = $"File '{record!.OriginalFileName}' uploaded successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Upload");
            }
        }

        // GET: /Files/Content/{id}?token=xxxx
        [HttpGet]
        public async Task<IActionResult> Content(Guid id, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Missing token.");

            var record = await _fileService.GetByIdAsync(id);
            if (record == null)
                return NotFound();

            // Signed URL check
            if (!string.Equals(record.SignedToken, token, StringComparison.Ordinal))
                return Unauthorized("Invalid token.");

            var stream = await _cloud.DownloadAsync(record.StoredFileName);
            return File(stream, record.ContentType, enableRangeProcessing: true);
        }
    }
}
