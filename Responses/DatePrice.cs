namespace StorageAPI.Responses
{
    public class DatePrice
    {
        public DateTime? PriceAt { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
    }
}
