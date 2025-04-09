using Domain.Entities;

namespace Domain.Interfaces;

public interface ICartShopDetailRepository
{
    Task<CartShopDetail> GetById(int id);
    Task<IEnumerable<CartShopDetail>> GetAll();
    Task<IEnumerable<CartShopDetail>> GetByCartId(int cartId);
    Task<CartShopDetail> Add(CartShopDetail entity);
    Task Update(CartShopDetail entity);
    Task Delete(int id);
}