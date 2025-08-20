using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models.Repositorio;
using Inmobiliaria.Models.Entidades;
namespace Inmobiliaria.Controllers
{
    public class InquilinosController : Controller
    {
      RepositorioInquilino repositorio;
      public InquilinosController(IConfiguration config)
      {
        repositorio = new RepositorioInquilino(config);
      }
        // GET: Inquilinos
         // [Authorize(Policy = "Empleado")]
        public ActionResult Index()
        {
            var lista= repositorio.ObtenerTodos();
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
        // [Authorize(Policy = "Empleado")]
        public ActionResult Create(Inquilino i)
        {
            try
          {
              int res= repositorio.Alta(i);
              if (res>0) {
              return RedirectToAction(nameof(Index));
              }
              else
               return View();            
            }
            catch(Exception e)
            {
               Console.WriteLine(e);
                return View();
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
        public ActionResult Edit(int id, Inquilino i )
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
            catch(Exception e)
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