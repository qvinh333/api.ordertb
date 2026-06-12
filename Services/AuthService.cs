using API.Sale.Data;
using API.Sale.DTOs.Auth;
using API.Sale.Models;
using API.Sale.Models.Enums;
using API.Sale.Utilities;

namespace API.Sale.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<User?> GetCurrentUserAsync(long userId);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == request.Username && u.IsActive);
        if (user == null)
        {
            return Task.FromResult<LoginResponse?>(null);
        }

        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Task.FromResult<LoginResponse?>(null);
        }

        var token = _tokenService.GenerateToken(user.Id, user.Username, user.Role.ToString());

        return Task.FromResult<LoginResponse?>(new LoginResponse
        {
            AccessToken = token,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role.ToString()
            }
        });
    }

    public Task LogoutAsync()
    {
        // JWT is stateless, so no server-side action needed
        // Client will simply remove the token
        return Task.CompletedTask;
    }

    public Task<User?> GetCurrentUserAsync(long userId)
    {
        return Task.FromResult<User?>(_context.Users.FirstOrDefault(u => u.Id == userId && u.IsActive));
    }
}



