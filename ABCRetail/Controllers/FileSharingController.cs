using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class FileSharingController : Controller
    {
        private readonly FileShareService _fileService;
        public FileSharingController(FileShareService fileService)
        {
          _fileService = fileService;
        }

        public async Task <IActionResult> Index()
        {
            var files = await _fileService.ListFilesAsync();  
            return View(files);
        }

        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("File name is required.");

            var stream = await _fileService.DownloadFileAsync(fileName);
            return File(stream, "application/octet-stream", fileName);
        }
        
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        // NEW: Handle file upload (POST)
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            using var stream = file.OpenReadStream();
            await _fileService.UploadFileAsync(file.FileName, stream);

            return RedirectToAction("Index"); // Go back to file list after upload
        }
    }
}
