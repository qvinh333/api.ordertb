namespace API.Sale.DTOs.Customer;

public class UpdateCustomerRequest
{
    public string? CustomerCode { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Note { get; set; }
}

