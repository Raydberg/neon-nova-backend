using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [MaxLength(500, ErrorMessage = "El nombre de la categoría no debe superar los 500 caracteres.")]
    public string Name { get; set; }

    [MaxLength(500, ErrorMessage = "La descripción de la categoría no debe superar los 500 caracteres.")]
    public string Description { get; set; }

    // Una categoría puede tener muchos productos.
    public ICollection<Product> Products { get; set; } = new List<Product>();
}