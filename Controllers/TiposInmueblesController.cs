using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models.Repositorio;
using Inmobiliaria.Models.Entidades;
namespace Inmobiliaria.Controllers
{
    public class TiposInmuebleController : Controller
    {
        private readonly RepositorioTipoInmueble repositorio;

        public TiposInmuebleController(RepositorioTipoInmueble repositorio)
        {
            this.repositorio = repositorio;
        }
    }
}