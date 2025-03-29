using Application.DTOs.ProductsDTOs;

namespace Application.Interfaces
{
    // Cumple el papel de Use Cases 
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync ();
        Task<ProductDto> GetByIdAsync (int id);
        Task AddAsync (CreateProductDto product);
        Task UpdateAsync (UpdateProductDto product);
        Task DeleteAsync (int id);
    }
}
