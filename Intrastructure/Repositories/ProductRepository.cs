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

    public async Task<PagedResult<Product>> GetAllPaginatedAsync(int pageNumber, int pageSize,
        ProductStatus? status = null)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderBy(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
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
                Punctuation = _context.ProductComments
                    .Where(c => c.ProductId == p.Id)
                    .Any()
                    ? (int)Math.Round(_context.ProductComments
                        .Where(c => c.ProductId == p.Id)
                        .Average(c => (double)c.Rating))
                    : 0,
                ImageUrl = p.Images.OrderBy(i => i.Id).FirstOrDefault()!.ImageUrl
            })
            .ToListAsync();
    }

    public async Task UpdateProductPunctuationAsync(int productId)
    {
        var avgRating = await _context.ProductComments
            .Where(c => c.ProductId == productId)
            .AverageAsync(c => c.Rating);

        var product = await _context.Products.FindAsync(productId);
        if (product != null)
        {
            product.Punctuation = (int)Math.Round(avgRating);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PagedResult<ProductSimplified>> GetAllProductSimplifiedPaginatedAsync(int pageNumber,
        int pageSize)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.Status == ProductStatus.Active);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var products = await query
            .OrderBy(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductSimplified
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name,
                CategoryId = p.CategoryId,
                Punctuation = _context.ProductComments
                    .Where(c => c.ProductId == p.Id)
                    .Any()
                    ? (int)Math.Round(_context.ProductComments
                        .Where(c => c.ProductId == p.Id)
                        .Average(c => (double)c.Rating))
                    : 0,
                ImageUrl = p.Images.OrderBy(i => i.Id).FirstOrDefault()!.ImageUrl
            })
            .ToListAsync();

        return new PagedResult<ProductSimplified>
        {
            Items = products,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}