namespace StorageAPI.Responses
{
    public class LatestItem
    {
        public string? Name { get; set; }
        public string? CarBaseUrl { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? LastPriceAt { get; set; }
        public decimal? LastPrice { get; set; }
    }
}
