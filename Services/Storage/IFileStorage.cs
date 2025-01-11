
using System.IO;
using System.Threading.Tasks;

namespace RealtyHub.Services.Storage
{
    /// <summary>
    /// Interface for file storage service [Local, Azure, AWS, etc.]
    /// </summary>
    public interface IFileStorage
    {
        Task<string> UploadAsync(string path, Stream stream);
        Task<Stream> DownloadAsync(string path);
        Task<bool> DeleteAsync(string path);
        Task<string>UpdatedAsync(string path, Stream stream);
    }

    
}