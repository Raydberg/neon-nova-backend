using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NeonNovaApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController (IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts () => Ok(await _productService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById (int id) => Ok(await _productService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> AddProduct (CreateProductDto productDto)
        {
            await _productService.AddAsync(productDto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct (UpdateProductDto productDto)
        {
            await _productService.UpdateAsync(productDto);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct (int id)
        {
            await _productService.DeleteAsync(id);
            return Ok();
        }

    }
}
