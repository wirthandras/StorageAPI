using Microsoft.EntityFrameworkCore;
using StorageAPI.Configuration;
using StorageAPI.Model;
using StorageAPI.Responses;

namespace StorageAPI.Services
{
    public class CarService : ICarService
    {
        private readonly AppDBContext appDBContext;
        public CarService(AppDBContext dbContect) 
        {
            appDBContext = dbContect;
        }

        public async Task<LatestChangesResponse> LatestChanges(int limit = 10)
        {
            var lastestChanges = await appDBContext.Price
                .Include(p => p.Car)
                .OrderByDescending(p => p.CreatedAt)
                .Take(limit)
                .ToListAsync();

            List<LatestItem> latestItems = new();

            lastestChanges.ForEach(c =>
            {
                latestItems.Add(new LatestItem
                {
                    Name = c?.Car?.Name,
                    LastPrice = c?.Price,
                    LastPriceAt = c?.CreatedAt,
                    CarBaseUrl = c?.Car?.CarBaseUrl,
                    ImageUrl = c?.Car?.ImageUrl,
                });
            });

            return new LatestChangesResponse
            {
                LatestChanges = latestItems.AsEnumerable()
            };
        }

        public Task Save(StorageRequest request)
        {
            var existingCar = appDBContext.Car
                .Include(c => c.Prices)
                .FirstOrDefault(f => f.ImageUrl == request.ImageUrl);

            if (existingCar is null)
            {
                appDBContext.Car.Add(new Car
                {
                    ImageUrl = request.ImageUrl,
                    Name = request.Name,
                    CarBaseUrl = request.CarBaseUrl
                });

                appDBContext.SaveChangesAsync();
            }
            else 
            {
                existingCar.CarBaseUrl = request.CarBaseUrl;
                if (request.Price.HasValue)
                {
                    if (existingCar.Prices.Count > 0)
                    {
                        var previousPrice = existingCar.Prices.OrderByDescending(o => o.CreatedAt).FirstOrDefault();

                        if (!previousPrice?.Price.Equals(request.Price) ?? false) 
                        {
                            existingCar.Prices.Add(new CarPrice
                            {
                                CreatedAt = DateTime.UtcNow,
                                Price = request.Price ?? 0,
                            });
                            appDBContext.SaveChangesAsync();
                        }
                    }
                    else 
                    {
                        existingCar.Prices.Add(new CarPrice
                        {
                            CreatedAt = DateTime.UtcNow,
                            Price = request.Price ?? 0,
                        });
                        appDBContext.SaveChangesAsync();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
