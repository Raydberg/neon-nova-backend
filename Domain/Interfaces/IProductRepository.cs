using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeletedAsync(int id);
        Task<Product> GetByIdWithCategoryAsync(int id);
        Task<IEnumerable<Product>> GetAllWithCategoriesAsync();
        Task<IEnumerable<ProductSimplified>> GetAllProductSimplifiedAsync();
    }
}