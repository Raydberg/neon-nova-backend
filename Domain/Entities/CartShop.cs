using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

public class CartShop
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El usuario es obligatorio para el carrito.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "La fecha de creación es obligatoria.")]
    public DateTime CreationDate { get; set; }

    [Required(ErrorMessage = "El estado del carrito es obligatorio.")]
    public CartStatus Status { get; set; }

    // Propiedades de navegación.
    public User User { get; set; }
    public ICollection<CartShopDetail> CartShopDetails { get; set; } = new List<CartShopDetail>();
}