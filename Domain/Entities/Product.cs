using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [MaxLength(500, ErrorMessage = "El nombre del producto no debe superar los 500 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La descripción del producto es obligatoria.")]
        [MaxLength(500, ErrorMessage = "La descripción del producto no debe superar los 500 caracteres.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El precio del producto es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El stock del producto es obligatorio.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La categoría del producto es obligatoria.")]
        public int CategoryId { get; set; }

        // Propiedad de navegación: cada producto pertenece a una categoría.
        public Category Category { get; set; }

        // Propiedades de navegación.
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductComment> Comments { get; set; } = new List<ProductComment>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<CartShopDetail> CartShopDetails { get; set; } = new List<CartShopDetail>();
    }
}