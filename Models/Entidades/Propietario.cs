using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models.Entidades;

public class Propietario
{
  [Display(Name = "Código")]
  public int Id { get; set; }
  [Required]
  public string? Nombre { get; set; }
  [Required]
  public string? Apellido { get; set; }
  [Display(Name = "DNI")]
  [RegularExpression(@"^\d{1,10}$", ErrorMessage = "El DNI debe contener solo números.")]
  [Required]
  public string? Dni { get; set; }
  [Display(Name = "Teléfono")]
  [RegularExpression(@"^\d{1,20}$", ErrorMessage = "El teléfono debe contener solo números.")]
  public string? Telefono { get; set; }
  [Required, EmailAddress]
  public string? Email { get; set; }


}
