using System.ComponentModel.DataAnnotations;
using Inmobiliaria.Models.Entidades;

namespace Inmobiliaria.Models.Entidades
{
  public class Contrato
  {
    [Display(Name = "Código")]
    public int Id { get; set; }
    [Required, Display(Name = "Fecha inicio")]
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; }
    [Required, Display(Name = "Fecha fin")]
    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; }
    [Required]
    public int Precio { get; set; }
    public string? Estado { get; set; }
    [Required]
    [Display(Name = "Inquilino")]
    public int InquilinoId { get; set; }
    [Display(Name = "Inmueble")]
    [Required]
    public int InmuebleId { get; set; }
    public Inquilino? Inquilino { get; set; }
    public Inmueble? Inmueble { get; set; }

    // [Display(Name = "DNI Garante")]
    // public string? Dni_Garante { get; set; }
    // [Display(Name = "Nombre Garante")]
    // public string? Nombre_Garante { get; set; }
    // [Display(Name = "Apellido Garante")]
    // public string? Apellido_Garante { get; set; }
    // [Display(Name = "Teléfono Garante")]
    // public string? Telefono_Garante { get; set; }
  }
}