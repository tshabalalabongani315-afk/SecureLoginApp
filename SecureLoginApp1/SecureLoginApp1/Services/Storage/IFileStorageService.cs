using System.IO;
using System.Threading.Tasks;

namespace SecureLoginApp1.Services.Storage
{
    /// <summary>
    /// Storage abstraction (Strategy pattern) so local dev and a future cloud provider
    /// can both plug in without touching calling code.
    /// </summary>
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream fileStream, string fileName, string contentType);

        Task DeleteAsync(string fileUrl);
    }
}
