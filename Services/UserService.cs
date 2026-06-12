using API.Sale.Data;
using API.Sale.DTOs.User;
using API.Sale.Models;
using API.Sale.Models.Enums;
using API.Sale.Utilities;

namespace API.Sale.Services;

public interface IUserService
{
    Task<(List<UserResponse> users, int total)> GetUsersAsync(int page = 1, int pageSize = 10, string? searchName = null, string? role = null);
    Task<User?> GetUserByIdAsync(long id);
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<User> UpdateUserAsync(long id, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(long id);
}

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public Task<(List<UserResponse> users, int total)> GetUsersAsync(int page = 1, int pageSize = 10, string? searchName = null, string? role = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(searchName))
        {
            query = query.Where(u => u.FullName.Contains(searchName) || u.Username.Contains(searchName));
        }

        if (!string.IsNullOrEmpty(role))
        {
            if (Enum.TryParse<UserRole>(role, true, out var userRole))
            {
                query = query.Where(u => u.Role == userRole);
            }
        }

        var total = query.Count();
        var users = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .ToList();

        return Task.FromResult((users, total));
    }

    public Task<User?> GetUserByIdAsync(long id)
    {
        return Task.FromResult<User?>(_context.Users.FirstOrDefault(u => u.Id == id));
    }

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        if (_context.Users.Any(u => u.Username == request.Username))
        {
            throw new InvalidOperationException("Username already exists");
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            FullName = request.FullName,
            Role = Enum.Parse<UserRole>(request.Role, true),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> UpdateUserAsync(long id, UpdateUserRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (!string.IsNullOrEmpty(request.FullName))
        {
            user.FullName = request.FullName;
        }

        if (!string.IsNullOrEmpty(request.Role))
        {
            user.Role = Enum.Parse<UserRole>(request.Role, true);
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> DeleteUserAsync(long id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
}

