using Application.DTOs.ProductsDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync ();
        Task<ProductDto> GetByIdAsync (int id);
        Task AddAsync (CreateProductDto product);
        Task UpdateAsync (UpdateProductDto product);
        Task DeleteAsync (int id);
    }
}
