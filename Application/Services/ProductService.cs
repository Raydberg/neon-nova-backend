using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Services;


public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IProductImageRepository _imageRepository;
    private readonly ICloudinaryService _cloudinary;
    private readonly IMapper _mapper;
    private readonly IProductImageService _productImageService;

    public ProductService(IProductRepository repository,IProductImageRepository imageRepository ,ICloudinaryService cloudinary, IMapper mapper,
        IProductImageService productImageService)
    {
        _repository = repository;
        _imageRepository = imageRepository;
        _cloudinary = cloudinary;
        _mapper = mapper;
        _productImageService = productImageService;
    }


    public async Task<List<ProductResponseDTO>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        var productDTOs = new List<ProductResponseDTO>();
    
        foreach (var product in products)
        {
            var dto = _mapper.Map<ProductResponseDTO>(product);
            var images = await _productImageService.GetImagesProductIdAsync(product.Id);
            dto.Images = _mapper.Map<List<ProductImageDTO>>(images);
            productDTOs.Add(dto);
        }

        return productDTOs;
    }

    public async Task<ProductResponseDTO> CreateAsync(CreateProductRequestDTO dto)
    {
        // Crea una versión del DTO sin la propiedad Images
        var productToCreate = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Status = dto.Status,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(productToCreate);
    
        // Subir imágenes a Cloudinary
        if (dto.Images != null)
        {
            foreach (var image in dto.Images)
            {
                await _productImageService.AddImageAsync(productToCreate.Id, image);
            }
        }

        return await GetByIdWithImagesAsync(productToCreate.Id);
    }

    public async Task<ProductResponseDTO> GetByIdWithImagesAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product is null)
        {
            throw new KeyNotFoundException("Producto no encontrado");
        }
        var images = await _productImageService.GetImagesProductIdAsync(id);
    
        return new ProductResponseDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Status = product.Status,
            // CreatedAt = product.CreatedAt,
            Images = _mapper.Map<List<ProductImageDTO>>(images)
        };
    }

    public async Task<ProductResponseDTO> UpdateAsync(int id, UpdateProductRequestDTO dto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product is null) throw new KeyNotFoundException();
        if (dto.Name != null) product.Name = dto.Name;
        if (dto.Price != null) product.Price = dto.Price.Value;
        if (dto.Status != null) product.Status = dto.Status.Value;

        await _repository.UpdateAsync(product);
        // Actualizar Imagen
        if (dto.Image != null)
        {
            var oldImage = await _productImageService.GetPrimaryImageAsync(id);
            if (oldImage != null)
            {
                await _productImageService.DeleteImageAsync(oldImage.Id);
            }

            //Subir la nueva imagen
            await _productImageService.AddImageAsync(id, dto.Image);
        }

        return await GetByIdWithImagesAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product is null) throw new KeyNotFoundException();
        var images = await _productImageService.GetImagesProductIdAsync(id);
        foreach (var image in images)
        {
            await _productImageService.DeleteImageAsync(image.Id);
        }

        await _repository.DeletedAsync(id);
    }

    public async Task<ProductImageDTO> UpdateImageAsync(int productId, int imageId, IFormFile image)
    {
        var existingImage = await _imageRepository.GetByIdAsync(imageId);
    
        if (existingImage == null || existingImage.ProductId != productId)
            throw new KeyNotFoundException("Imagen no encontrada o no pertenece al producto indicado");

        await _cloudinary.DeleteImage(existingImage.PublicId);

        var imageUrl = await _cloudinary.UploadImage(image);
        var publicId = _cloudinary.GetPublicIdFromUrl(imageUrl);

        existingImage.ImageUrl = imageUrl;
        existingImage.PublicId = publicId;

        await _imageRepository.UpdateAsync(existingImage);

        return _mapper.Map<ProductImageDTO>(existingImage);
    }
}