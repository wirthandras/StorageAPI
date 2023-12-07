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

        public async Task<KeyWordResponse> GetKeyWords()
        {
            var carNames = await appDBContext.Car
                .Select(x => x.Name)
                .ToListAsync();

            var keyWords = carNames.Select(name => name?.Split(' ').FirstOrDefault()).Distinct().ToList();

            return new KeyWordResponse
            {
                KeyWords = keyWords.AsEnumerable()
            };
        }

        public async Task<LatestChangesResponse> LatestChanges(int limit = 10, string? search = "")
        {
            IQueryable<CarPrice> query = appDBContext.Price
                .Include(p => p.Car);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Car != null && c.Car.Name != null && c.Car.Name.Contains(search));
            }

            query = query.OrderByDescending(p => p.CreatedAt)
                .Take(limit);

            var lastestChanges = await query.ToListAsync();

            var priceChanges = await appDBContext.Price
                .Include(p => p.Car)
                .OrderBy(p => p.CreatedAt)
                .Where(x => lastestChanges.Select(s => s.CarId).Contains(x.CarId))
                .ToListAsync();

            List<LatestItem> latestItems = new();

            lastestChanges.ForEach(c =>
            {
                List<decimal?> minList = new();
                if (c is not null)
                {
                    minList.Add(c.Price);
                    if (c.DiscountedPrice.HasValue)
                    {
                        minList.Add(c.DiscountedPrice.Value);
                    }
                }

                latestItems.Add(new LatestItem
                {
                    Name = c?.Car?.Name,
                    LastPrice = minList.Min(),
                    LastPriceAt = c?.CreatedAt,
                    CarBaseUrl = c?.Car?.CarBaseUrl,
                    ImageUrl = c?.Car?.ImageUrl,
                    Prices = priceChanges
                                .Where(p => p.CarId == c?.CarId)
                                .Select(s => new DatePrice
                                {
                                    PriceAt = s.CreatedAt,
                                    Price = s.Price,
                                    DiscountedPrice = s.DiscountedPrice
                                })
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

                        bool hasNewPrice = !previousPrice?.Price.Equals(request.Price) ?? false;
                        bool hasNewDiscountedPrice = !previousPrice?.DiscountedPrice.Equals(request.DiscountedPrice) ?? false;

                        if (hasNewPrice || hasNewDiscountedPrice)
                        {
                            existingCar.Prices.Add(new CarPrice
                            {
                                CreatedAt = DateTime.UtcNow,
                                Price = request.Price ?? 0,
                                DiscountedPrice = request.DiscountedPrice,
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
                            DiscountedPrice = request.DiscountedPrice ?? 0,
                        });
                        appDBContext.SaveChangesAsync();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
