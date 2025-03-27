using Application.DTOs.ProductsDTOs;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public class ProductMapper: Profile
    {
        public ProductMapper ()
        {
            // Mapear de Product hacia ProductDTO 
            CreateMap<Product,ProductDto>();
            CreateMap<CreateProductDto,Product>();
            CreateMap<UpdateProductDto,Product>();

        }

    }
}
