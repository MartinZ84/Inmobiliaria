using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models.Repositorio;
using Inmobiliaria.Models.Entidades;
namespace Inmobiliaria.Controllers
{
  public class InquilinosController : Controller
  {
    private readonly RepositorioInquilino repositorio;

    public InquilinosController(RepositorioInquilino repositorio)
    {
      this.repositorio = repositorio;
    }
    // GET: Inquilinos
    // [Authorize(Policy = "Empleado")]
    // public ActionResult Index()
    // {
    //   var lista = repositorio.ObtenerTodos();
    //   return View(lista);
    // }
    
public ActionResult Index(string? dni = null, string? nombre = null, 
    string? apellido = null, string? email = null, int pagina=1)
{
    try
    {
        IList<Inquilino> inquilinos;
        var tamaño = 5;
        // Si hay filtros, usar el método de búsqueda
        if (!string.IsNullOrWhiteSpace(dni) || !string.IsNullOrWhiteSpace(nombre) || 
            !string.IsNullOrWhiteSpace(apellido) || !string.IsNullOrWhiteSpace(email))
        {
            inquilinos = repositorio.BuscarInquilinosConValidacion(dni, nombre, apellido, email);
            var total = inquilinos.Count;
            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
            // Mensaje informativo si no hay resultados
            if (inquilinos.Count == 0)
            {
                TempData["InfoMessage"] = "No se encontraron inquilinos con los criterios especificados.";
                TempData["AlertType"] = "info";
            }
            else if (inquilinos .Count >= 200)
            {
                TempData["WarningMessage"] = "Se encontraron muchos resultados (200+). Considere refinar su búsqueda.";
                TempData["AlertType"] = "warning";
            }
        }
        else
        {
            // Sin filtros, obtener todos los inquilinos           
            inquilinos = repositorio.ObtenerLista(pagina, 5);
            ViewBag.Pagina = pagina;
				    var total = repositorio.ObtenerCantidad();
				    ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
        }

        // Pasar los filtros a la vista para mantenerlos en el formulario
        ViewBag.Dni = dni;
        ViewBag.Nombre = nombre;
        ViewBag.Apellido = apellido;
        ViewBag.Email = email;

        return View(inquilinos);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error en Index: {ex}");
        TempData["ErrorMessage"] = "Ocurrió un error al cargar los propietarios.";
        TempData["AlertType"] = "danger";
        return View(new List<Propietario>());
    }
}

    // GET: Inquilinos/Details/5
    // [Authorize(Policy = "Empleado")]
    public ActionResult Details(int id)
    {
      var inquilino = repositorio.ObtenerPorId(id);
      return View(inquilino);
    }

    // GET: Inquilinos/Create
    // [Authorize(Policy = "Empleado")]
    public ActionResult Create()
    {
      return View();
    }

    // POST: Inquilinos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Inquilino inquilino)
    {
      try
      {
        // Verifica si el DNI ya existe
        string? errorDni = repositorio.ExisteDniInquilino(inquilino.Dni);

        if (!string.IsNullOrEmpty(errorDni))
        {
          TempData["ErrorMessage"] = errorDni;
          TempData["AlertType"] = "danger"; // Para Bootstrap alert-danger
          return View(inquilino);
        }

        // Si el DNI es válido, procede a guardar
        int res = repositorio.Alta(inquilino);
        if (res > 0)
        {
          TempData["SuccessMessage"] = "Inquilino creado exitosamente";
          TempData["AlertType"] = "success";
          return RedirectToAction(nameof(Index));
        }
        else
        {
          TempData["ErrorMessage"] = "Error al crear el inquilino";
          TempData["AlertType"] = "danger";
          return View(inquilino);
        }
      }
      catch (Exception e)
      {
        TempData["ErrorMessage"] = "Error inesperado al procesar la solicitud";
        TempData["AlertType"] = "danger";
        Console.WriteLine(e);
        return View(inquilino);
      }
    }
    // GET: Inquilinos/Edit/5
    // [Authorize(Policy = "Empleado")]
    public ActionResult Edit(int id)
    {
      try
      {
        var inquilino = repositorio.ObtenerPorId(id);
        return View(inquilino);
        //pasa el modelo a la vista            
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }

    // POST: Inquilinos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Authorize(Policy = "Empleado")]
    public ActionResult Edit(int id, Inquilino i)
    {
      // Inquilino? inquilinoEdit;
      try
      {

        // Obtener el inquilino original de la base de datos
        var inquilinoOriginal = repositorio.ObtenerPorId(id);
        if (inquilinoOriginal == null)
        {
          TempData["ErrorMessage"] = "El inquilino no existe.";
          TempData["AlertType"] = "danger";
          return RedirectToAction(nameof(Index));
        }

        // Validar DNI solo si cambió
        if (i.Dni != inquilinoOriginal.Dni)
        {
          string? errorDni = repositorio.ExisteDniInquilino(i.Dni);
          if (!string.IsNullOrEmpty(errorDni))
          {
            TempData["ErrorMessage"] = errorDni;
            TempData["AlertType"] = "danger";
            return View(i);
          }
        }

        repositorio.Modificacion(i);
        TempData["Mensaje"] = "Datos guardados correctamente";
        return RedirectToAction(nameof(Index));
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        return View();
      }
    }

    // GET: Inquilinos/Delete/5
    // [Authorize(Policy = "Empleado")]
    public ActionResult Delete(int id)
    {
      try
      {
        var inquilino = repositorio.ObtenerPorId(id);
        if (TempData.ContainsKey("Mensaje"))
          ViewBag.Mensaje = TempData["Mensaje"];
        if (TempData.ContainsKey("Error"))
          ViewBag.Error = TempData["Error"];
        return View(inquilino);
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }
    }

    // POST: Inquilinos/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    // [Authorize(Policy = "Empleado")]
    public ActionResult Delete(int id, Inquilino i)
    {
      try
      {
        repositorio.Baja(id);
        TempData["Mensaje"] = "Eliminación realizada correctamente";
        return RedirectToAction(nameof(Index));
      }
      catch (Exception ex)
      {
        var inq = repositorio.ObtenerPorId(id);
        ViewBag.Error = ex.Message;
        ViewBag.StackTrate = ex.StackTrace;
        return View(inq);
      }
    }
    
     public ActionResult Buscar(string? dni, string? nombre, string? apellido, string? email)
    {
      try
      {
        var propietarios = repositorio.BuscarInquilinosConValidacion(dni, nombre, apellido, email);

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