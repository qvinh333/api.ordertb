using System.Security.Claims;

namespace API.Sale.Services;

public class CurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long GetUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (long.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return 0;
    }

    public string GetUsername()
    {
        return _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }

    public string GetRole()
    {
        return _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    public bool IsAdmin()
    {
        return GetRole() == "ADMIN";
    }

    public bool IsSeller()
    {
        return GetRole() == "SELLER";
    }
}

