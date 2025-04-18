using Domain.Entities;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Domain.Interfaces;

public class ProductCommentRepository : IProductCommentRepository
{
    private readonly ApplicationDbContext _context;
    public ProductCommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ProductComment> AddAsync(ProductComment comment)
    {
        await _context.ProductComments.AddAsync(comment);
        await _context.SaveChangesAsync();
        return comment;
    }
    public async Task<ProductComment> UpdateAsync(ProductComment comment)
    {
        _context.ProductComments.Update(comment);
        await _context.SaveChangesAsync();
        return comment;
    }
    public async Task<ProductComment> GetByIdAsync(int id)
    {
        return await _context.ProductComments
            .Include(c => c.Product)  // Incluye el producto relacionado
            .Include(c => c.User)     // Incluye el usuario relacionado, no "Users"
            .FirstOrDefaultAsync(c => c.Id == id); // Buscar por el Id del comentario
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var comment = await _context.ProductComments.FindAsync(id);
        if (comment == null) return false;

        _context.ProductComments.Remove(comment);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> ExistsByProductAndUserAsync(int productId, string userId)
    {
        return await _context.ProductComments
            .AnyAsync(c => c.ProductId == productId && c.UserId == userId);
    }
    public async Task<ProductComment?> GetByProductAndUserAsync(int productId, string userId)
    {
        return await _context.ProductComments
            .Include(c => c.Product)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);
    }
    public async Task<List<ProductComment>> GetCommentsByProductIdAsync(int productId)
    {
        return await _context.ProductComments
            .Where(c => c.ProductId == productId)
            .ToListAsync();
    }
  
    
    
}