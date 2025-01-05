
namespace RealtyHub.Services.Storage
{
    public interface IFileStorage
    {
        Task<string> UploadAsync();
        Task<Stream> DownloadAsync();
        Task<bool> DeleteAsync();
        Task<string>UpdatedAsync();
    }
}