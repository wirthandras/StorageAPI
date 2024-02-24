namespace StorageAPI.Model
{
    public class Car
    {
        public long Id { get; set; }
        public int BrandId { get; set; }
        public string? Name { get; set; }
        public string? CarBaseUrl { get; set; }
        public string? ImageUrl { get; set; }
        public int? Year { get; set; }
        public int? Mileage { get; set; }
        public int? CubicCapacity { get; set; }
        public virtual ICollection<CarPrice> Prices { get; } = new List<CarPrice>();
        public virtual ICollection<Image> Images { get; } = new List<Image>();
        public virtual Brand? Brand { get; set; }
    }
}
