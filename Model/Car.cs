namespace StorageAPI.Model
{
    public class Car
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<CarPrice> Prices { get; } = new List<CarPrice>();
    }
}
