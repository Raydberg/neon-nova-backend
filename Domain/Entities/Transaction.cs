using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

public class Transaction
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La orden es obligatoria para la transacción.")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "La fecha de pago es obligatoria.")]
    public DateTime PaymentDate { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio.")]
    public float Amount { get; set; }

    [Required(ErrorMessage = "El método de pago es obligatorio.")]
    public PaymentMethod PaymentMethod { get; set; }

    [Required(ErrorMessage = "El estado de la transacción es obligatorio.")]
    public PaymentStatus PaymentStatus { get; set; }

    // Propiedad de navegación.
    public Order Order { get; set; }
}