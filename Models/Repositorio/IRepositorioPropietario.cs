using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models.Entidades;

namespace Inmobiliaria.Models.Repositorio
{
	public interface IRepositorioPropietario : IRepositorio<Propietario>
	{
		Propietario ObtenerPorEmail(string email);
		IList<Propietario> BuscarPorNombre(string nombre);
		IList<Propietario> ObtenerLista(int paginaNro, int tamPagina);
		int ObtenerCantidad();
	}
}
