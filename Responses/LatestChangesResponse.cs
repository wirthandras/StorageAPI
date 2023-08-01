using StorageAPI.Model;

namespace StorageAPI.Responses
{
    public class LatestChangesResponse
    {
        public IEnumerable<LatestItem> LatestChanges { get; set; } = Enumerable.Empty<LatestItem>();
    }
}
