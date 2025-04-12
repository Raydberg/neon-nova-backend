using Application.DTOs.ProductsDTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDTO>> GetAllAsync();
    Task<ProductResponseDTO> CreateAsync(CreateProductRequestDTO dto);
    Task<ProductResponseDTO> GetByIdWithImagesAsync(int id);
    Task<ProductResponseDTO> UpdateAsync(int id, UpdateProductRequestDTO dto);
    Task DeleteAsync(int id);

    Task<ProductImageDTO> UpdateImageAsync(int productId, int imageId, IFormFile image);
}