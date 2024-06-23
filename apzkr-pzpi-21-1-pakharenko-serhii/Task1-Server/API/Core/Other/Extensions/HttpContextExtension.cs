namespace API.Core.Extensions;

public static class HttpContextExtension
{
    public static bool HasGuardRole(this HttpContext httpContext)
    {
        var isGuard = httpContext.Items["IsGuard"] as bool? ?? false;
        return isGuard;
    }
}
