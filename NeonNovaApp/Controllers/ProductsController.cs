using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers;

[Route("/api/product")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;

    public ProductController(IProductService productService, IProductImageService productImageService)
    {
        _productService = productService;
        _productImageService = productImageService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateProductRequestDTO dto)
    {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await _productService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdWithImagesAsync(id);
        return product == null ? NotFound() : Ok(product);
    }


    [HttpPut("{productId}/images/{imageId}")]
    public async Task<IActionResult> UpdateImage(int productId, int imageId, [FromForm] UpdateProductImageDTO dto)
    {
        var updatedImage = await _productService.UpdateImageAsync(productId, imageId, dto.Image);
        return Ok(updatedImage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImage(int id, IFormFile image)
    {
        await _productImageService.AddImageAsync(id, image);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateProductRequestDTO dto)
    {
        try
        {
            var updateProduct = await _productService.UpdateAsync(id, dto);
            return Ok(updateProduct);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    [HttpDelete("{productId}/images/{imageId}")]
    public async Task<IActionResult> DeleteImage(int productId, int imageId)
    {
        try
        {
            var images = await _productImageService.GetImagesProductIdAsync(productId);
            if (!images.Any(img => img.Id == imageId))
            {
                return NotFound("La imagen no pertenece al producto especificado");
            }

            await _productImageService.DeleteImageAsync(imageId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Imagen no encontrada");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar la imagen: {ex.Message}");
        }
    }
}