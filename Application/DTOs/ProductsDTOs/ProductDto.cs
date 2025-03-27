using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductsDTOs
{

    public record ProductDto (int Id, string Name, decimal Price);

}
