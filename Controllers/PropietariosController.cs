using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models.Repositorio;
using Inmobiliaria.Models.Entidades;

namespace Inmobiliaria.Controllers
{
  public class PropietariosController : Controller
  {
    private readonly RepositorioPropietario repositorio;

    public PropietariosController(RepositorioPropietario repositorio)
    {
      this.repositorio = repositorio;
    }
    // GET: Propietarios
    // [Authorize(Policy = "Empleado")]
    // public ActionResult Index()
    // {
    //   var lista = repositorio.ObtenerTodos();
    //   return View(lista);
    // }

    // Método Index actualizado para manejar filtros
    public ActionResult Index(string? dni = null, string? nombre = null,
        string? apellido = null, string? email = null, int pagina = 1)
    {
      try
      {
        IList<Propietario> propietarios;
        var tamaño = 10;
        // Si hay filtros, usar el método de búsqueda
        if (!string.IsNullOrWhiteSpace(dni) || !string.IsNullOrWhiteSpace(nombre) ||
            !string.IsNullOrWhiteSpace(apellido) || !string.IsNullOrWhiteSpace(email))
        {
          propietarios = repositorio.BuscarPropietariosConValidacion(dni, nombre, apellido, email);
          var total = propietarios.Count;
          ViewBag.Pagina = pagina;
          ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
          // Mensaje informativo si no hay resultados
          if (total == 0)
          {
            TempData["InfoMessage"] = "No se encontraron propietarios con los criterios especificados.";
            TempData["AlertType"] = "info";
          }
          else if (total >= 200)
          {
            TempData["WarningMessage"] = "Se encontraron muchos resultados (200+). Considere refinar su búsqueda.";
            TempData["AlertType"] = "warning";
          }
        }
        else
        {
          // Sin filtros, obtener todos los propietarios
          // propietarios = repositorio.ObtenerTodos();
          propietarios = repositorio.ObtenerLista(pagina, tamaño);
          ViewBag.Pagina = pagina;
          var total = repositorio.ObtenerCantidad();
          ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
        }

        // Pasar los filtros a la vista para mantenerlos en el formulario
        ViewBag.Dni = dni;
        ViewBag.Nombre = nombre;
        ViewBag.Apellido = apellido;
        ViewBag.Email = email;

        return View(propietarios);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error en Index: {ex}");
        TempData["ErrorMessage"] = "Ocurrió un error al cargar los propietarios.";
        TempData["AlertType"] = "danger";
        return View(new List<Propietario>());
      }
    }

    // GET: Propietarios/Details/5
    // [Authorize(Policy = "Empleado")]
    public ActionResult Details(int id)
    {
      var propietario = repositorio.ObtenerPorId(id);
      return View(propietario);
    }
    public ActionResult ObtenerPropPorId(int id)
    {
      try
      {
        var propietario = repositorio.ObtenerPorId(id);
        if (propietario == null)
          return Json(new { Error = "No se encontró el propietario" });

        // Para Select2, el texto visible se pone en 'text'
        var res = new
        {
          id = propietario.Id,
          text = $"{propietario.Nombre} {propietario.Apellido}"
        };

        return Json(res);
      }
      catch (Exception ex)
      {
        return Json(new { Error = ex.Message });
      }
    }


    // GET: Propietarios/Create
    // [Authorize(Policy = "Empleado")]
    public ActionResult Create()
    {
      return View();
    }

    // POST: Propietarios/Create

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Propietario p)
    {
      try
      {
        // Verifica si el DNI ya existe
        string? errorDni = repositorio.ExisteDniPropietario(p.Dni);

        if (!string.IsNullOrEmpty(errorDni))
        {
          TempData["ErrorMessage"] = errorDni;
          TempData["AlertType"] = "danger"; // Para Bootstrap alert-danger
          return View(p);
        }

        // Si el DNI es válido, procede a guardar
        int res = repositorio.Alta(p);
        if (res > 0)
        {
          TempData["SuccessMessage"] = "Propietario creado exitosamente";
          TempData["AlertType"] = "success";
          return RedirectToAction(nameof(Index));
        }
        else
        {
          TempData["ErrorMessage"] = "Error al crear el propietario";
          TempData["AlertType"] = "danger";
          return View(p);
        }
      }
      catch (Exception e)
      {
        TempData["ErrorMessage"] = "Error inesperado al procesar la solicitud";
        TempData["AlertType"] = "danger";
        Console.WriteLine(e);
        return View(p);
      }
    }

    // GET: Propietarios/Edit/5
    // [Authorize(Policy = "Empleado")]
    public ActionResult Edit(int id)
    {
      try
      {
        var prop = repositorio.ObtenerPorId(id);
        return View(prop);
        //pasa el modelo a la vista

      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }


    }

    // POST: Propietarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    // // [Authorize(Policy = "Empleado")]
    public ActionResult Edit(int id, Propietario p)
    {
      // Propietario ? propEdit= null;
      try
      {


        // Obtener el inquilino original de la base de datos
        var propietariooOriginal = repositorio.ObtenerPorId(id);
        if (propietariooOriginal == null)
        {
          TempData["ErrorMessage"] = "El inquilino no existe.";
          TempData["AlertType"] = "danger";
          return RedirectToAction(nameof(Index));
        }

        // Validar DNI solo si cambió
        if (p.Dni != propietariooOriginal.Dni)
        {
          string? errorDni = repositorio.ExisteDniPropietario(p.Dni);
          if (!string.IsNullOrEmpty(errorDni))
          {
            TempData["ErrorMessage"] = errorDni;
            TempData["AlertType"] = "danger";
            return View(p);
          }
        }

        repositorio.Modificacion(p);
        TempData["Mensaje"] = "Datos guardados correctamente";

        return RedirectToAction(nameof(Index));
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        return View();
      }
    }

    // GET: Propietarios/Delete/5
    // [Authorize(Policy = "Empleado")]
    public ActionResult Delete(int id)
    {
      try
      {
        var prop = repositorio.ObtenerPorId(id);
        if (TempData.ContainsKey("Mensaje"))
          ViewBag.Mensaje = TempData["Mensaje"];
        if (TempData.ContainsKey("Error"))
          ViewBag.Error = TempData["Error"];
        return View(prop);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }

    }

    // POST: Propietarios/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Authorize(Policy = "Empleado")]
    public ActionResult Delete(int id, Propietario propietario)
    {
      try
      {
        repositorio.Baja(id);
        TempData["Mensaje"] = "Eliminación realizada correctamente";
        return RedirectToAction(nameof(Index));
      }
      catch (Exception ex)
      {
        var prop = repositorio.ObtenerPorId(id);
        ViewBag.Error = ex.Message;
        ViewBag.StackTrate = ex.StackTrace;
        return View(prop);
        // return View();
      }
    }



    // GET: Propietario/Buscar/5
    [Route("[controller]/Buscar", Name = "Buscar")]
    public IActionResult Buscar(string q)
    {
      try
      {
        var res = repositorio.ObtenerPorNombre(q);
        return Json(new { Datos = res });
      }
      catch (Exception ex)
      {
        return Json(new { Error = ex.Message });
      }
    }



    public ActionResult Buscar(string? dni, string? nombre, string? apellido, string? email)
    {
      try
      {
        var propietarios = repositorio.BuscarPropietariosConValidacion(dni, nombre, apellido, email);

        if (propietarios.Count == 0)
        {
          if (string.IsNullOrWhiteSpace(dni) && string.IsNullOrWhiteSpace(nombre) &&
              string.IsNullOrWhiteSpace(apellido) && string.IsNullOrWhiteSpace(email))
          {
            TempData["InfoMessage"] = "Debe especificar al menos un criterio de búsqueda.";
          }
          else
          {
            TempData["InfoMessage"] = "No se encontraron propietarios con los criterios especificados.";
          }
        }
        else if (propietarios.Count >= 200)
        {
          TempData["WarningMessage"] = "Se encontraron muchos resultados (200+). Considere refinar su búsqueda.";
        }

        ViewBag.Dni = dni;
        ViewBag.Nombre = nombre;
        ViewBag.Apellido = apellido;
        ViewBag.Email = email;

        return View(propietarios);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error en búsqueda: {ex}");
        TempData["ErrorMessage"] = "Ocurrió un error durante la búsqueda.";
        return View(new List<Propietario>());
      }
    }

  }
}