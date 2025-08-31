using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models.Entidades
{
    public class TipoInmueble
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [StringLength(100, ErrorMessage = "Descripcion debe ser menor a 100 caracteres")]
        public string Descripcion { get; set; }

    }
}