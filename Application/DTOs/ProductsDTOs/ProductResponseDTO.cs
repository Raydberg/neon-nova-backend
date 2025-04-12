using Domain.Enums;

namespace Application.DTOs.ProductsDTOs
{
    public record ProductResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public ProductStatus Status { get; set; }
        public List<ProductImageDTO> Images { get; set; } = new();
    }
}
