using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dffbackend.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AdminApiKeyAttribute : Attribute, IAuthorizationFilter
{
    private const string API_KEY_HEADER_NAME = "X-Factor-API-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var submittedApiKey = GetSubmittedApiKey(context.HttpContext);

        if (!CheckAdminApiKey(submittedApiKey, context.HttpContext))
        {
            context.Result = new UnauthorizedResult();
        }
        else
        {
            var claimId = new Claim(ClaimTypes.Sid, "admin");
            var claimName = new Claim(ClaimTypes.Name, "admin");
            var claimEmail = new Claim(ClaimTypes.Email, "admin@pks.rs");
            var identity = new ClaimsIdentity(new[] { claimId, claimName, claimEmail }, "ApiKeyAuthentication");
            var principal = new ClaimsPrincipal(identity);
            context.HttpContext.User = principal;
        }
    }

    private static string GetSubmittedApiKey(HttpContext context)
    {
        return context.Request.Headers[API_KEY_HEADER_NAME];
    }

    private bool CheckAdminApiKey(string submittedApiKey, HttpContext context)
    {
        if (string.IsNullOrEmpty(submittedApiKey)) return false;

        var configuration = context.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;

        return configuration["AdminApiKey"] == submittedApiKey;
    }
}