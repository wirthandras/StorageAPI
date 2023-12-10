namespace StorageAPI.Model
{
    public class StorageRequest
    {
        public string? Name { get; set; }
        public int? Year { get; set; }
        public int? Mileage { get; set; }
        public int? CubicCapacity { get; set; }
        public string? CarBaseUrl { get; set; }
        public string? ImageUrl { get; set;}
        public decimal? Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
    }
}
