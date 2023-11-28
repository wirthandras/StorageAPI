using Google.Apis.Auth;
using StorageAPI.Controllers;

public class GoogleTokenValidator : ITokenValidator
{
    private readonly IConfiguration _configuration;

    public GoogleTokenValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> ValidateTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _configuration["Google:Audience"] },
                
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
