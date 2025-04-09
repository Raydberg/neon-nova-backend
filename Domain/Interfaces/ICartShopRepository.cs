using Domain.Entities;

namespace Domain.Interfaces;

public interface ICartShopRepository
{
    Task<CartShop> GetById(int id);
    Task<IEnumerable<CartShop>> GetAll();
    Task<CartShop> GetByUser(int userId);
    Task<CartShop> Add(CartShop entity);
    Task Update(CartShop entity);
    Task Delete(int id);
}