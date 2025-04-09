using Domain.Entities;

namespace Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category> GetById(int id);
    Task<IEnumerable<Category>> GetAll();
    Task<IEnumerable<Product>> GetProducts(int categoryId);
    Task<Category> Add(Category entity);
    Task Update(Category entity);
    Task Delete(int id);
}