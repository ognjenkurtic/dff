using System.Security.Claims;
using dffbackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dffbackend.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class FactoringCompanyApiKeyAttribute : Attribute, IAuthorizationFilter
{
    private const string API_KEY_HEADER_NAME = "X-Factor-API-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var submittedApiKey = GetSubmittedApiKey(context.HttpContext);

        var factoringCompany = GetFactoringCompanyBasedOnApiKey(submittedApiKey, context.HttpContext);

        if (factoringCompany is null)
        {
            context.Result = new UnauthorizedResult();
        }
        else
        {
            var claimId = new Claim(ClaimTypes.Sid, factoringCompany.Id.ToString());
            var claimName = new Claim(ClaimTypes.Name, factoringCompany.Name);
            var claimEmail = new Claim(ClaimTypes.Email, factoringCompany.Email);
            var identity = new ClaimsIdentity(new[] { claimId, claimName, claimEmail }, "ApiKeyAuthentication");
            var principal = new ClaimsPrincipal(identity);
            context.HttpContext.User = principal;
        }
    }

    private static string GetSubmittedApiKey(HttpContext context)
    {
        return context.Request.Headers[API_KEY_HEADER_NAME];
    }

    private FactoringCompany GetFactoringCompanyBasedOnApiKey(string submittedApiKey, HttpContext context)
    {
        if (string.IsNullOrEmpty(submittedApiKey)) return null;

        var dbContext = context.RequestServices.GetService(typeof(DffContext)) as DffContext;

        return dbContext.FactoringCompanies.FirstOrDefault(fc => fc.ApiKey == submittedApiKey && !fc.IsDeleted.Value);
    }
}