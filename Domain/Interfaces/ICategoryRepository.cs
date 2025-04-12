using Domain.Entities;

namespace Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category> GetByIdAsync(int id);
    Task<List<Category>> GetAllAsync();
    Task<List<Product>> GetProductsAsync(int categoryId);
    Task<Category> AddAsync(Category entity);
    Task<Category> UpdateAsync(Category entity);
    Task DeleteAsync(int id);
}