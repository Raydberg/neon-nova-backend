using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AuthDTOs;

public class CredentialsUserDto
{
    [Required(ErrorMessage = "El correo es obligatorio")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    public string? Password { get; set; }
}