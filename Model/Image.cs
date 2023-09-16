namespace StorageAPI.Model
{
    public class Image
    {
        public long Id { get; set; }
        public long CarId { get; set; }
        public virtual Car? Car { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ExternalUrl { get; set; }
        public string? InternalUrl { get; set; }
    }
}
