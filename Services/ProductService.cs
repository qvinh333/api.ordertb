using API.Sale.Data;
using API.Sale.DTOs.Product;
using API.Sale.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Sale.Services;

public interface IProductService
{
    Task<(List<ProductResponse> products, int total)> GetProductsAsync(long currentUserId, int page = 1, int pageSize = 10, string? search = null);
    Task<Product?> GetProductByIdAsync(long id, long currentUserId);
    Task<Product> CreateProductAsync(CreateProductRequest request, long currentUserId);
    Task<Product> UpdateProductAsync(long id, UpdateProductRequest request, long currentUserId);
    Task<bool> DeleteProductAsync(long id, long currentUserId);
}

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public Task<(List<ProductResponse> products, int total)> GetProductsAsync(long currentUserId, int page = 1, int pageSize = 10, string? search = null)
    {
        var query = _context.Products.Where(p => !p.Deleted && p.CreatedBy == currentUserId).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchPattern = $"%{search.Trim()}%";
            query = query.Where(p =>
                EF.Functions.ILike(AppDbContext.Unaccent(p.ProductCode), AppDbContext.Unaccent(searchPattern)) ||
                EF.Functions.ILike(AppDbContext.Unaccent(p.Name), AppDbContext.Unaccent(searchPattern)));
        }

        var total = query.Count();
        var products = query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToResponse)
            .ToList();

        return Task.FromResult((products, total));
    }

    public Task<Product?> GetProductByIdAsync(long id, long currentUserId)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id && !p.Deleted && p.CreatedBy == currentUserId);
        return Task.FromResult(product);
    }

    public async Task<Product> CreateProductAsync(CreateProductRequest request, long currentUserId)
    {
        if (_context.Products.Any(p => p.ProductCode == request.ProductCode && !p.Deleted && p.CreatedBy == currentUserId))
        {
            throw new InvalidOperationException("Product code already exists");
        }

        var product = new Product
        {
            ProductCode = request.ProductCode,
            Name = request.Name,
            Specification = request.Specification,
            Unit = request.Unit,
            DefaultSellingPrice = request.DefaultSellingPrice,
            Note = request.Note,
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Deleted = false
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<Product> UpdateProductAsync(long id, UpdateProductRequest request, long currentUserId)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id && !p.Deleted && p.CreatedBy == currentUserId);
        if (product == null)
        {
            throw new InvalidOperationException("Product not found");
        }

        if (!string.IsNullOrWhiteSpace(request.ProductCode) && request.ProductCode != product.ProductCode)
        {
            if (_context.Products.Any(p => p.ProductCode == request.ProductCode && !p.Deleted && p.CreatedBy == currentUserId))
            {
                throw new InvalidOperationException("Product code already exists");
            }

            product.ProductCode = request.ProductCode;
        }

        if (!string.IsNullOrWhiteSpace(request.Name)) product.Name = request.Name;
        if (request.Specification != null) product.Specification = request.Specification;
        if (request.Unit != null) product.Unit = request.Unit;
        if (request.DefaultSellingPrice.HasValue) product.DefaultSellingPrice = request.DefaultSellingPrice;
        if (request.Note != null) product.Note = request.Note;

        product.UpdatedAt = DateTime.UtcNow;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<bool> DeleteProductAsync(long id, long currentUserId)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id && !p.Deleted && p.CreatedBy == currentUserId);
        if (product == null)
        {
            return false;
        }

        product.Deleted = true;
        product.UpdatedAt = DateTime.UtcNow;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return true;
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            ProductCode = product.ProductCode,
            Name = product.Name,
            Specification = product.Specification,
            Unit = product.Unit,
            DefaultSellingPrice = product.DefaultSellingPrice,
            Note = product.Note,
            CreatedBy = product.CreatedBy,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}

