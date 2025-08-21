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
    public ActionResult Index()
    {
      var lista = repositorio.ObtenerTodos();
      return View(lista);
    }

    // GET: Propietarios/Details/5
    // [Authorize(Policy = "Empleado")]
    public ActionResult Details(int id)
    {
      var propietario = repositorio.ObtenerPorId(id);
      return View(propietario);
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

        // propEdit = repositorio.ObtenerPorId(id);
        // propEdit.Nombre=p.Nombre;
        // propEdit.Apellido=p.Apellido;
        // propEdit.Dni=p.Dni;
        // propEdit.Email=p.Email;
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
    [Route("[controller]/Buscar/{q}", Name = "Buscar")]
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
  }
}