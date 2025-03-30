using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(500, ErrorMessage = "El nombre no debe superar los 500 caracteres.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [MaxLength(500, ErrorMessage = "El apellido no debe superar los 500 caracteres.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [MaxLength(500, ErrorMessage = "El correo electrónico no debe superar los 500 caracteres.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MaxLength(500, ErrorMessage = "La contraseña no debe superar los 500 caracteres.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [Phone(ErrorMessage = "El teléfono no es válido.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio.")]
    public int RoleId { get; set; }

    // Propiedad de navegación: cada usuario tiene un rol.
    public Role Role { get; set; }

    // Colecciones de navegación.
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<ProductComment> Comments { get; set; } = new List<ProductComment>();
    public ICollection<CartShop> Carts { get; set; } = new List<CartShop>();
}