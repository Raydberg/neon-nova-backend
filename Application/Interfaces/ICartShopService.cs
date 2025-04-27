using Application.DTOs.CartShopDTOs;

namespace Application.Interfaces;

public interface ICartShopService
{
    Task<CartShopDto> GetCurrentCartAsync();
    Task<CartShopDto> AddToCartAsync(AddToCartShopDto dto);
    Task<CartShopDto> UpdateCartItemAsync(UpdateCartShopItemDto dto);
    Task<CartShopDto> RemoveCartItemAsync(int cartDetailId);
    Task<bool> ClearCartAsync();
    Task<bool> CompleteCartAsync();
    Task<CartShopDto> RefreshCartAsync();
}