using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ProductImage
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El producto es obligatorio para la imagen.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "La URL de la imagen es obligatoria.")]
    [MaxLength(500, ErrorMessage = "La URL de la imagen no debe superar los 500 caracteres.")]
    public string ImageUrl { get; set; }

    [MaxLength(500, ErrorMessage = "El texto alternativo no debe superar los 500 caracteres.")]
    public string AltText { get; set; }

    // Propiedad de navegación.
    public Product Product { get; set; }
}