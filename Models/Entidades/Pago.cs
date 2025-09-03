using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models.Entidades;

  public class Pago{

  [Display(Name= "Código")]     
  public int Id { get; set; }

    [Display(Name= "Numero de pago")]     
    public int NroPago { get; set; }

   [Required,Display(Name="Fecha pago")]
    [DataType(DataType.Date)]    
    public DateTime FechaPago { get; set; }
   [Required]
    public Decimal Importe {get;set;}

    [Display(Name= "Contrato")]     
     [Required]
   public int ContratoId { get; set; }
     
    public Contrato? Contrato  { get; set; }
    
  }
