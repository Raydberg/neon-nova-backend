using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync ();
        Task<Product> GetAsync (int id);
        Task DeleteAsync (int id);
        Task AddAsync (Product product);
        Task UpdateAsync (Product product);

    }
}
