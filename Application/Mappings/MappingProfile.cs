using Application.DTOs.ProductsDTOs;
using AutoMapper;
using Domain.Entities;


namespace Application.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile ()
        {
            // Mapear de Product hacia ProductDTO 
            CreateMap<Product,ProductDto>();
            CreateMap<CreateProductDto,Product>();
            CreateMap<UpdateProductDto,Product>();

        }

    }
}
