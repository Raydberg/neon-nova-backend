using Application.DTOs.ProductsDTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        public ProductService (IProductRepository productRepository, IMapper mapper)
        {
            _repository = productRepository;
            _mapper = mapper;

        }

        public async Task AddAsync (CreateProductDto product)
        {
            await _repository.AddAsync(_mapper.Map<Product>(product));
        }

        public async Task DeleteAsync (int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<List<ProductDto>> GetAllAsync () => _mapper.Map<List<ProductDto>>(await _repository.GetAllAsync());

        public async Task<ProductDto> GetByIdAsync (int id) => _mapper.Map<ProductDto>(await _repository.GetAsync(id));

        public async Task UpdateAsync (UpdateProductDto product)
        {
           await _repository.UpdateAsync(_mapper.Map<Product>(product));
        }
    }
}
