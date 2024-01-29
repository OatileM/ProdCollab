using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProdCollab.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ProdCollab.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly S3Service _s3Service;

        public HomeController(ILogger<HomeController> logger, S3Service s3Service)
        {
            _logger = logger;
            _s3Service = s3Service;
        }

        public IActionResult Index()
        {
            // Log information
            _logger.LogInformation("Accessed Index action.");
            return View();
        }

        public IActionResult Privacy()
        {
            // Log information
            _logger.LogInformation("Accessed Privacy action.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                // Log a warning
                _logger.LogWarning("Received null or empty file for upload.");
                return RedirectToAction("Error");
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    await _s3Service.UploadFileAsync("prodcollab", file.FileName, stream);
                }

                // Log a success message
                _logger.LogInformation($"File '{file.FileName}' uploaded successfully.");

                // Add file information to the database or a list of files.

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                _logger.LogError(ex, "An error occurred during file upload");

                // Provide a user-friendly error message
                ViewBag.ErrorMessage = "An error occurred while uploading the file. Please try again later.";
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                // Log a warning
                _logger.LogWarning("Invalid file name for download.");
                return BadRequest("Invalid file name.");
            }

            try
            {
                var fileStream = await _s3Service.DownloadFileAsync("prodcollab", fileName);

                if (fileStream == null)
                {
                    // Log a warning
                    _logger.LogWarning($"File '{fileName}' not found for download.");
                    return NotFound();
                }

                // Log a success message
                _logger.LogInformation($"File '{fileName}' downloaded successfully.");

                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                _logger.LogError(ex, "An error occurred during file download");

                // Provide a user-friendly error message
                ViewBag.ErrorMessage = "An error occurred while downloading the file. Please try again later.";
                return RedirectToAction("Error");
            }
        }
    }
}
