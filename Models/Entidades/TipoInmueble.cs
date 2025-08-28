using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models.Entidades
{
    public class TipoInmueble
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; }

    }
}