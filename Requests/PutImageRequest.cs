namespace StorageAPI.Requests
{
    public class PutImageRequest
    {
        public string? ExternalImageUrl { get; set; }

        public string? InternalImageUrl { get; set; }
    }
}
