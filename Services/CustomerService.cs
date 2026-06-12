using API.Sale.Data;
using API.Sale.DTOs.Customer;
using API.Sale.Models;

namespace API.Sale.Services;

public interface ICustomerService
{
    Task<(List<CustomerResponse> customers, int total)> GetCustomersAsync(long currentUserId, int page = 1, int pageSize = 10, string? search = null);
    Task<Customer?> GetCustomerByIdAsync(long id, long currentUserId);
    Task<Customer> CreateCustomerAsync(CreateCustomerRequest request, long currentUserId);
    Task<Customer> UpdateCustomerAsync(long id, UpdateCustomerRequest request, long currentUserId);
    Task<bool> DeleteCustomerAsync(long id, long currentUserId);
}

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public Task<(List<CustomerResponse> customers, int total)> GetCustomersAsync(long currentUserId, int page = 1, int pageSize = 10, string? search = null)
    {
        var query = _context.Customers.Where(c => !c.Deleted && c.CreatedBy == currentUserId).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.CustomerCode.Contains(search) || c.FullName.Contains(search) || (c.Phone != null && c.Phone.Contains(search)));
        }

        var total = query.Count();
        var customers = query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToResponse)
            .ToList();

        return Task.FromResult((customers, total));
    }

    public Task<Customer?> GetCustomerByIdAsync(long id, long currentUserId)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Id == id && !c.Deleted && c.CreatedBy == currentUserId);
        return Task.FromResult(customer);
    }

    public async Task<Customer> CreateCustomerAsync(CreateCustomerRequest request, long currentUserId)
    {
        if (_context.Customers.Any(c => c.CustomerCode == request.CustomerCode && !c.Deleted && c.CreatedBy == currentUserId))
        {
            throw new InvalidOperationException("Customer code already exists");
        }

        var customer = new Customer
        {
            CustomerCode = request.CustomerCode,
            FullName = request.FullName,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            Note = request.Note,
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Deleted = false
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return customer;
    }

    public async Task<Customer> UpdateCustomerAsync(long id, UpdateCustomerRequest request, long currentUserId)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Id == id && !c.Deleted && c.CreatedBy == currentUserId);
        if (customer == null)
        {
            throw new InvalidOperationException("Customer not found");
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerCode) && request.CustomerCode != customer.CustomerCode)
        {
            if (_context.Customers.Any(c => c.CustomerCode == request.CustomerCode && !c.Deleted && c.CreatedBy == currentUserId))
            {
                throw new InvalidOperationException("Customer code already exists");
            }

            customer.CustomerCode = request.CustomerCode;
        }

        if (!string.IsNullOrWhiteSpace(request.FullName)) customer.FullName = request.FullName;
        if (request.Phone != null) customer.Phone = request.Phone;
        if (request.Email != null) customer.Email = request.Email;
        if (request.Address != null) customer.Address = request.Address;
        if (request.Note != null) customer.Note = request.Note;

        customer.UpdatedAt = DateTime.UtcNow;

        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();

        return customer;
    }

    public async Task<bool> DeleteCustomerAsync(long id, long currentUserId)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Id == id && !c.Deleted && c.CreatedBy == currentUserId);
        if (customer == null)
        {
            return false;
        }

        customer.Deleted = true;
        customer.UpdatedAt = DateTime.UtcNow;

        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();

        return true;
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            CustomerCode = customer.CustomerCode,
            FullName = customer.FullName,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            Note = customer.Note,
            CreatedBy = customer.CreatedBy,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}

