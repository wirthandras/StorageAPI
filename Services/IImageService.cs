using StorageAPI.Requests;

namespace StorageAPI.Services
{
    public interface IImageService
    {
        Task CreateNewImage(PostImageRequest request);

        Task UpdateInternalImage(PutImageRequest request);
    }
}
