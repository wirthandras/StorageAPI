using StorageAPI.Configuration;
using StorageAPI.Model;
using StorageAPI.Requests;

namespace StorageAPI.Services
{
    public class ImageService : IImageService
    {
        private readonly AppDBContext appDBContext;
        public ImageService(AppDBContext dbContect)
        {
            appDBContext = dbContect;
        }

        public async Task CreateNewImage(PostImageRequest request)
        {
            var car = appDBContext.Car.FirstOrDefault(c => c.CarBaseUrl == request.CarBaseUrl);

            if (car is not null) {
                await appDBContext.Image.AddAsync(new Image
                {
                    CreatedAt = DateTime.UtcNow,
                    ExternalUrl = request.Url,
                    CarId = car.Id
                });
                await appDBContext.SaveChangesAsync();
            }

            return;
        }

        public async Task UpdateInternalImage(PutImageRequest request)
        {
            var image = appDBContext.Image.FirstOrDefault(c => c.ExternalUrl == request.ExternalImageUrl);

            if (image is not null) {
                image.InternalUrl = request.InternalImageUrl;
                await appDBContext.SaveChangesAsync();
            }

            return;
        }
    }
}
