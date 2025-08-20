using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models.Entidades;

public class Inquilino
{
  [Key]
  [Display(Name= "Código")]
  [Required]
  public int Id { get ;set; }
  [Required]
  public string? Nombre { get ;set; }
  [Required]
  public string? Apellido { get ;set; }
  
  [Display(Name = "DNI")]
  [RegularExpression(@"^\d{1,10}$", ErrorMessage = "El DNI debe contener solo números.")]
  [Required]
  public string? Dni { get ;set; }
  [Display(Name= "Teléfono")]
  [RegularExpression(@"^\d{1,20}$", ErrorMessage = "El teléfono debe contener solo números.")]
  [Required]
  public string? Telefono { get ;set; }
  [Required,EmailAddress]
  public string? Email { get ;set; }

}
