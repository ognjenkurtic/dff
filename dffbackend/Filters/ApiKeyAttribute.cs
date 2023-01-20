using System.Runtime.InteropServices;
using System.Security.Cryptography;
using dffbackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace dffbackend.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class ApiKeyAttribute : Attribute, IAuthorizationFilter
{
    private const string API_KEY_HEADER_NAME = "X-Factor-API-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var submittedApiKey = GetSubmittedApiKey(context.HttpContext);

        // var apiKey = GetApiKey(context.HttpContext);

        // TODO: apply the commented code below in a following manner:
        // From configuration (env variable) fetch encryption/decryption password
        // Encrypt key and check against DB
        // Use IsApiKeyValid method to validate api key and replace the IsApiKeyValidInDb call below

        if (!IsApiKeyValidInDb(submittedApiKey, context.HttpContext))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private bool IsApiKeyValidInDb(string submittedApiKey, HttpContext context)
    {
        if (string.IsNullOrEmpty(submittedApiKey)) return false;

        var dbContext = context.RequestServices.GetService(typeof(DffContext)) as DffContext;

        return dbContext.FactoringCompanies.Any(fc => fc.ApiKey == submittedApiKey);
    }

    private static string GetSubmittedApiKey(HttpContext context)
    {
        return context.Request.Headers[API_KEY_HEADER_NAME];
    }

    // private static string GetApiKey(HttpContext context)
    // {
    //     var configuration = context.RequestServices.GetRequiredService<IConfiguration>();

    //     return configuration.GetValue<string>($"ApiKey");
    // }

    // private static bool IsApiKeyValid(string apiKey, string submittedApiKey)
    // {
    //     if (string.IsNullOrEmpty(submittedApiKey)) return false;

    //     var apiKeySpan = MemoryMarshal.Cast<char, byte>(apiKey.AsSpan());

    //     var submittedApiKeySpan = MemoryMarshal.Cast<char, byte>(submittedApiKey.AsSpan());

    //     return CryptographicOperations.FixedTimeEquals(apiKeySpan, submittedApiKeySpan);
    // }
}