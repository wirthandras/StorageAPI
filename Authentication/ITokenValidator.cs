namespace StorageAPI.Controllers
{
    public interface ITokenValidator
    {
        Task<bool> ValidateTokenAsync(string idToken);
    }
}
