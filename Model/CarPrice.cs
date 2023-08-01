namespace StorageAPI.Model
{
    public class CarPrice
    {
        public long Id { get; set; }
        public long CarId { get; set; }
        public virtual Car? Car { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }
    }
}
