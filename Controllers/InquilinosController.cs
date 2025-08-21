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
    public ActionResult Index()
    {
      var lista = repositorio.ObtenerTodos();
      return View(lista);
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
        // inquilinoEdit = repositorio.ObtenerPorId(id);
        // var inq=  inquilinoEdit as Inquilino;
        //   inq.Nombre=i.Nombre;
        //   inq.Apellido=i.Apellido;
        //   inq.Dni=i.Dni;
        //   inq.Email=i.Email;
        //   inq.Lugar_Trabajo=i.Lugar_Trabajo;
        //   inq.Nombre_Garante=i.Nombre_Garante;
        //   inq.Apellido_Garante=i.Apellido_Garante;
        //   inq.Telefono_Garante=i.Telefono_Garante;
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
  }
}