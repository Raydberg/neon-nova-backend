using Domain.Entities;
using Domain.Enums;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Domain.Interfaces;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Attach(product);
        var entry = _context.Entry(product);
        entry.Property(p => p.Name).IsModified = true;
        entry.Property(p => p.Description).IsModified = true;
        entry.Property(p => p.Price).IsModified = true;
        entry.Property(p => p.Stock).IsModified = true;
        entry.Property(p => p.CategoryId).IsModified = true;
        entry.Property(p => p.Status).IsModified = true;
        await _context.SaveChangesAsync();
    }

    public async Task DeletedAsync(int id)
    {
        var product = await GetByIdAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Product> GetByIdWithCategoryAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllWithCategoriesAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductSimplified>> GetAllProductSimplifiedAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.Status == ProductStatus.Active)
            .Select(p => new ProductSimplified
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name,
                CategoryId = p.CategoryId,
                Punctuation = p.Punctuation,
                ImageUrl = p.Images.OrderBy(i => i.Id).FirstOrDefault()!.ImageUrl
            })
            .ToListAsync();
    }
}