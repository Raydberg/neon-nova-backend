using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync ();
        Task<Product> GetAsync (int id);
        Task DeleteAsync (int id);
        Task AddAsync (Product product);
        Task UpdateAsync (Product product);

    }
}
