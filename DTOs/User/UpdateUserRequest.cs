namespace API.Sale.DTOs.User;

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}

