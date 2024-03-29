﻿using Microsoft.EntityFrameworkCore;
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
                .Include(p => p.Car)
                .Where(p => p.Car != null && p.Car.EndOfSale == null);

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

            lastestChanges.DistinctBy(d => d.CarId).ToList().ForEach(c =>
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
                                    DiscountedPrice = s.DiscountedPrice.HasValue && s.DiscountedPrice.Value > decimal.Zero ? s.DiscountedPrice : s.Price
                                })
                });
            });

            return new LatestChangesResponse
            {
                LatestChanges = latestItems.AsEnumerable()
            };
        }

        public async Task ReCheckCars()
        {
            var cars = appDBContext.Car.Where(p => p.EndOfSale == null);

            using (var httpClient = new HttpClient())
            {
                foreach (var car in cars)
                {
                    if (car.CarBaseUrl is not null)
                    {
                        var response = await httpClient.GetAsync(car.CarBaseUrl);
                        if (!response.IsSuccessStatusCode)
                            car.EndOfSale = DateTime.UtcNow;
                    }
                }
            }
            await appDBContext.SaveChangesAsync();
            return;
        }

        public Task Save(StorageRequest request)
        {
            var existingBrand = appDBContext.Brand
                .FirstOrDefault(b => b.Name == request.Brand);

            var existingCar = appDBContext.Car
                .Include(c => c.Prices)
                .FirstOrDefault(f => f.ImageUrl == request.ImageUrl);

            if (existingCar is null)
            {
                var newCar = new Car
                {
                    Brand = existingBrand,
                    ImageUrl = request.ImageUrl,
                    Name = request.Name,
                    CarBaseUrl = request.CarBaseUrl,
                    Year = request.Year,
                    CubicCapacity = request.CubicCapacity,
                    Mileage = request.Mileage
                };

                appDBContext.Car.Add(newCar);

                newCar.Prices.Add(new CarPrice
                {
                    CreatedAt = DateTime.UtcNow,
                    Price = request.Price ?? 0,
                    DiscountedPrice = request.DiscountedPrice,
                });

                appDBContext.SaveChangesAsync();
            }
            else
            {
                existingCar.CarBaseUrl = request.CarBaseUrl;
                existingCar.EndOfSale = null;
                if (request.Year.HasValue && request.Year > 1990) 
                {
                    existingCar.Year = request.Year;
                }
                if (request.CubicCapacity.HasValue && request.CubicCapacity > 0)
                {
                    existingCar.CubicCapacity = request.CubicCapacity;
                }
                if (request.Mileage.HasValue && request.Mileage > 0)
                {
                    existingCar.Mileage = request.Mileage;
                }
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
