namespace StorageAPI.Model
{
    public class Brand
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Car> Cars { get; } = new List<Car>();
    }
}
