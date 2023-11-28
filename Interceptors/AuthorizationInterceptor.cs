using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StorageAPI.Controllers;

public class AuthorizationInterceptor : IAsyncActionFilter
{
    private readonly ILogger<AuthorizationInterceptor> _logger;

    public AuthorizationInterceptor(ILogger<AuthorizationInterceptor> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var authService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidator>();

        var auth = context.HttpContext.Request.Headers["Authorization"].ToString();
        string token = auth.Replace("Bearer ", "");
        var isTokenValid = await authService.ValidateTokenAsync(token);

        if (isTokenValid)
        {
            await next();
        }
        _logger.LogWarning("Invalid auth token");
        context.Result = new UnauthorizedResult();
    }
}
