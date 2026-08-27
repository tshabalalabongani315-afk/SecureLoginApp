using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SecureLoginApp1.Services.Storage
{
    /// <summary>
    /// Saves uploads to wwwroot/uploads on the local disk.
    /// </summary>
    public class LocalFileStorageService : IFileStorageService
    {
        private const string UploadsFolder = "uploads";

        private readonly IWebHostEnvironment _environment;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveAsync(Stream fileStream, string fileName, string contentType)
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, UploadsFolder);
            Directory.CreateDirectory(uploadsPath);

            var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var fullPath = Path.Combine(uploadsPath, storedFileName);

            using (var output = File.Create(fullPath))
            {
                await fileStream.CopyToAsync(output);
            }

            return $"/{UploadsFolder}/{storedFileName}";
        }

        public Task DeleteAsync(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return Task.CompletedTask;
            }

            var relativePath = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_environment.WebRootPath, relativePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }
    }
}
