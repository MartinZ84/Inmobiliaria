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
  [Required]
  public string? Dni { get ;set; }
  [Display(Name= "Teléfono")]
  [Required]
  public string? Telefono { get ;set; }
  [Required,EmailAddress]
  public string? Email { get ;set; }

}
