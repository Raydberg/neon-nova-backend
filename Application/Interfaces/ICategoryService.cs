using Application.DTOs.CategoryDTOs;
using Application.DTOs.ProductsDTOs;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto> GetByIdAsync(int id);
    Task<CategoryDto> AddCategoryAsync(CreateCategoryRequestDto category);
    Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequestDto category);
    Task<List<ProductDto>> GetProductsByCategoryAsync(int category);
    Task DeleteCategoryAsync(int id);
}