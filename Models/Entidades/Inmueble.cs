using System.ComponentModel.DataAnnotations;
using Inmobiliaria.Models.Enums;

namespace Inmobiliaria.Models.Entidades
{
    public class Inmueble
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(100, ErrorMessage = "La dirección no puede superar los 100 caracteres")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = "";

        [Required(ErrorMessage = "La cantidad de ambientes es obligatoria")]
        [Range(1, 20, ErrorMessage = "Los ambientes deben estar entre 1 y 20")]
        [Display(Name = "Ambientes")]
        public int Ambientes { get; set; }

        [Range(1, 10000, ErrorMessage = "La superficie debe ser mayor a 0")]
        [Display(Name = "Superficie (m²)")]
        public decimal? Superficie { get; set; }

        // [Required(ErrorMessage = "El tipo es obligatorio")]
        // [StringLength(50, ErrorMessage = "El tipo no puede superar los 50 caracteres")]
        // [Display(Name = "Tipo")]
        // public string Tipo { get; set; } = "";

        [Required(ErrorMessage = "El tipo de inmueble es obligatorio")]
        [Display(Name = "Tipo de Inmueble")]
        public int? TipoInmId { get; set; }
        
        public TipoInmueble? TipoInmueble { get; set; }


        [Required(ErrorMessage = "El uso es obligatorio")]
        [StringLength(20, ErrorMessage = "El uso no puede superar los 20 caracteres")]
        [Display(Name = "Uso")]
        public string Uso { get; set; } = "";

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Display(Name = "Precio")]
        public int Precio { get; set; }

        [Display(Name = "Latitud")]
        public decimal? Latitud { get; set; }

        [Display(Name = "Longitud")]
        public decimal? Longitud { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        // [StringLength(50, ErrorMessage = "El estado no puede superar los 50 caracteres")]
        [Display(Name = "Estado")]
        // public string Estado { get; set; } = "";        
        public EstadoInmueble Estado { get; set; }   
        public int EstadoBd { get; set; }   

        [Required(ErrorMessage = "El propietario es obligatorio")]
        [Display(Name = "Propietario")]
        public int PropietarioId { get; set; }

        // Propiedad de navegación
        public Propietario? Propietario { get; set; }

        // Propiedad para las imágenes (separadas por coma)
        [Display(Name = "Imágenes")]
        public string? Imagenes { get; set; }

        // Propiedad helper para trabajar con lista de imágenes
        public List<string> ListaImagenes
        {
            get
            {
                if (string.IsNullOrEmpty(Imagenes))
                    return new List<string>();
                
                return Imagenes.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(img => img.Trim())
                              .Where(img => !string.IsNullOrEmpty(img))
                              .ToList();
            }
        }

        // Método helper para obtener la primera imagen
        public string PrimeraImagen
        {
            get
            {
                var imagenes = ListaImagenes;
                return imagenes.Any() ? imagenes.First() : "/images/no-image.png";
            }
            set
            {
                if (ListaImagenes.Count > 0)
                {
                    ListaImagenes[0] = value;
                }
            }
        }

            // Nueva propiedad calculada para la vista
    public string PrimeraImagenDisplay => string.IsNullOrWhiteSpace(PrimeraImagen) 
                                           ? "/images/no-image.png"  // placeholder
                                           : PrimeraImagen.Trim();

        // Método helper para verificar disponibilidad
        public bool EstaDisponible => Estado == EstadoInmueble.Disponible;

    }
}