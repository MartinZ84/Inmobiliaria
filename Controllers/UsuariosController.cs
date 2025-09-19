using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inmobiliaria.Models;
using Inmobiliaria.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Inmobiliaria.Controllers
{
    public class UsuariosController : Controller
    {
        private Models.Repositorio.RepositorioUsuario repositorio;

        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        public UsuariosController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
            repositorio = new Models.Repositorio.RepositorioUsuario(configuration);

        }
        // GET: Usuarios
        [Authorize(Policy = "Administrador")]
        public ActionResult Index()
        {
            var usuarios = repositorio.ObtenerTodos();
            return View(usuarios);

        }

        // GET: Usuarios/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            var usuario = repositorio.ObtenerPorId(id);

            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(usuario);
        }

        // GET: Usuarios/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        // POST: Usuarios/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Usuario usuario)

        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: usuario.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                usuario.Clave = hashed;
                usuario.Rol = User.IsInRole("SuperAdministrador") ? usuario.Rol : (int)enRoles.Empleado;
                var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                int res = repositorio.Alta(usuario);
                if (usuario.AvatarFile != null && usuario.Id > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                    string fileName = "avatar_" + usuario.Id + Path.GetExtension(usuario.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    usuario.Avatar = Path.Combine("/Uploads", fileName);
                    // Esta operación guarda la foto en memoria en el ruta que necesitamos
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        usuario.AvatarFile.CopyTo(stream);
                    }
                    repositorio.Modificacion(usuario);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();
            }
        }


        // GET: Usuarios/Perfil/5
        [Authorize]
        public ActionResult Perfil()
        {
            ViewData["Title"] = "Mi perfil";
            //var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            var u = repositorio.ObtenerPorId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            if (u == null)
                return RedirectToAction("Login", "Usuarios");
            TempData.Remove("returnUrl");
            var returnUrl = "/Usuarios/Perfil";
            TempData["returnUrl"] = returnUrl;
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View("Edit", u);
        }


        // GET: Usuarios/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar usuario";
            var usuario = repositorio.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            TempData.Remove("returnUrl");
            var returnUrl = "/Usuarios/Edit/" + id;
            TempData["returnUrl"] = returnUrl;
            return View(usuario);
        }



        // POST: Usuarios/Edit/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // [Authorize]
        // public ActionResult Edit(int id, Usuario usuario)
        // {
        //     var vista = "";// nameof(Edit);//de que vista provengo
        //     var returnUrl = TempData["returnUrl"].ToString();
        //     try
        //     {
        //         if (!User.IsInRole("Administrador"))//no soy admin
        //         {
        //             vista = nameof(Perfil);//solo puedo ver mi perfil
        //             // var usuarioActual = repositorio.ObtenerPorEmail(User.Identity.Name);
        //             //if (usuarioActual.Id != id)//si no es admin, solo puede modificarse él mismo
        //             //  if (usuarioActual.Email != User.Identity.Name)
        //             //     return RedirectToAction(nameof(Index), "Home");

        //             //ACA INICIO MODIFICACION DESDE EL PERFIL DE USUARIO
        //             //else 
        //             if (usuario.Clave == null)//si es el formulario de edicion de los datos sin la clave
        //             {
        //                 // var usuarioActual = repositorio.ObtenerPorId(id);
        //                 var usuarioActual = repositorio.ObtenerPorEmail(User.Identity.Name);
        //                 usuario.Clave = usuarioActual.Clave;
        //                 usuario.Id = usuarioActual.Id;
        //                 usuario.Rol = usuarioActual.Rol;
        //                 //Identifico Si viene con archivo de avatar nuevo
        //                 if (usuario.AvatarFile != null && usuario.Id > 0)
        //                 {
        //                     string wwwPath = environment.WebRootPath;
        //                     string path = Path.Combine(wwwPath, "Uploads");
        //                     if (!Directory.Exists(path))
        //                     {
        //                         Directory.CreateDirectory(path);
        //                     }
        //                     //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
        //                     string fileName = "avatar_" + usuario.Id + Path.GetExtension(usuario.AvatarFile.FileName);
        //                     string pathCompleto = Path.Combine(path, fileName);
        //                     usuario.Avatar = Path.Combine("/Uploads", fileName);
        //                     // Esta operación guarda la foto en memoria en el ruta que necesitamos
        //                     using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
        //                     {
        //                         usuario.AvatarFile.CopyTo(stream);
        //                     }
        //                     repositorio.Modificacion(usuario);
        //                     // ViewBag.Roles = Usuario.ObtenerRoles();
        //                     // return RedirectToAction(vista);
        //                     // fin de edicion de avatar

        //                 }  //else { //si no viene con archivo de avatar obtengo los datos del avatar del usuario guardado y luego guardo
        //                 usuario.Avatar = usuarioActual.Avatar;
        //                 repositorio.Modificacion(usuario);
        //                 ViewBag.Roles = Usuario.ObtenerRoles();
        //                 return RedirectToAction(vista);
        //                 //  }

        //             }// Fin edicion de datos sin clave

        //             else //entra por el formulario de actualizacion de clave              
        //             {
        //                 string claveNueva;
        //                 var usuarioActual = repositorio.ObtenerPorEmail(User.Identity.Name);
        //                 string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //                     password: usuario.Clave,
        //                     salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
        //                     prf: KeyDerivationPrf.HMACSHA1,
        //                     iterationCount: 1000,
        //                     numBytesRequested: 256 / 8));
        //                 claveNueva = hashed;
        //                 repositorio.ModificacionClave(usuarioActual.Id, claveNueva);
        //                 ViewBag.Roles = Usuario.ObtenerRoles();
        //                 return RedirectToAction(vista);
        //             }//FIN EDICION DE CLAVE
        //         }//FIN DE EDICION DESDE EL PERFIL DE USUARIO

        //         //soy admin , identifico si viene a por form de datos o form clave
        //         else if (usuario.Clave == null)//si es el formulario de edicion de los datos sin la clave
        //         {
        //             var usuarioActual = repositorio.ObtenerPorId(id);
        //             usuario.Clave = usuarioActual.Clave;
        //             //Identifico Si viene con archivo de avatar nuevo
        //             if (usuario.AvatarFile != null && usuario.Id > 0)
        //             {
        //                 string wwwPath = environment.WebRootPath;
        //                 string path = Path.Combine(wwwPath, "Uploads");
        //                 if (!Directory.Exists(path))
        //                 {
        //                     Directory.CreateDirectory(path);
        //                 }
        //                 //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
        //                 string fileName = "avatar_" + usuario.Id + Path.GetExtension(usuario.AvatarFile.FileName);
        //                 string pathCompleto = Path.Combine(path, fileName);
        //                 usuario.Avatar = Path.Combine("/Uploads", fileName);
        //                 // Esta operación guarda la foto en memoria en el ruta que necesitamos
        //                 using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
        //                 {
        //                     usuario.AvatarFile.CopyTo(stream);
        //                 }
        //                 repositorio.Modificacion(usuario);
        //                 ViewBag.Roles = Usuario.ObtenerRoles();
        //                 //return Redirect(returnUrl);
        //                 // fin de edicion de avatar

        //             }  //else { //si no viene con archivo de avatar obtengo los datos del avatar del usuario guardado y luego guardo
        //                // usuario.Avatar = usuarioActual.Avatar;
        //             repositorio.Modificacion(usuario);
        //             // return RedirectToAction(vista);
        //             ViewBag.Roles = Usuario.ObtenerRoles();
        //             // return View(usuario);
        //             return Redirect(returnUrl);
        //             //  }

        //         }// Fin edicion de datos sin clave

        //         else //entra por el formulario de actualizacion de clave              
        //         {
        //             string claveNueva;
        //             string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //                 password: usuario.Clave,
        //                 salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
        //                 prf: KeyDerivationPrf.HMACSHA1,
        //                 iterationCount: 1000,
        //                 numBytesRequested: 256 / 8));
        //             claveNueva = hashed;
        //             repositorio.ModificacionClave(id, claveNueva);
        //             usuario = repositorio.ObtenerPorId(id);
        //             // return RedirectToAction(returnUrl, usuario);
        //             //  return RedirectToAction("returnUrl");
        //             return Redirect(returnUrl);

        //         }



        //         // return RedirectToAction(vista);
        //     }//del try
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //         return View();
        //     }
        // }
        //         [HttpPost]
        //         [ValidateAntiForgeryToken]
        //         [Authorize]

        // public ActionResult Edit(int id, Usuario usuario)
        // {
        //     var vista = ""; // Nombre de la vista de retorno
        //     var returnUrl = TempData["returnUrl"]?.ToString() ?? "/";

        //     try
        //     {
        //         // Obtener usuario actual desde la DB
        //         var usuarioExistente = repositorio.ObtenerPorId(id);
        //         if(usuario.Email != usuarioExistente.Email)
        //         {
        //             // Si se está cambiando el email, verificar que no exista otro usuario con ese email
        //             var usuarioConMismoEmail = repositorio.ObtenerPorEmail(usuario.Email);
        //             if (usuarioConMismoEmail != null && usuarioConMismoEmail.Id != id)
        //             {
        //                 ModelState.AddModelError("Email", "El email ya está en uso por otro usuario.");
        //                 ViewBag.Roles = Usuario.ObtenerRoles();
        //                 return View(usuario);
        //             }

        //         }

        //         //  Seguridad: validar que el usuario logueado solo edite su propio perfil si no es admin
        //                 var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        //         if (!User.IsInRole("Administrador") && usuarioExistente.Id != userId)
        //         {
        //             // Intento de modificar a otro usuario → acceso denegado
        //             return RedirectToAction("Restringido", "Home");
        //         }

        //         // Si el usuario NO es admin       
        //         if (!User.IsInRole("Administrador"))
        //         {
        //             vista = nameof(Perfil);

        //             // Rol siempre se conserva (no puede tocarlo)
        //             usuario.Rol = usuarioExistente.Rol;

        //             // Avatar
        //             if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
        //             {
        //                 GuardarAvatar(usuario);
        //             }
        //             else
        //             {
        //                 usuario.Avatar = usuarioExistente.Avatar;
        //             }

        //             // Clave: si se envió una nueva, la encripto; si no, mantengo la actual
        //             if (!string.IsNullOrEmpty(usuario.Clave))
        //             {
        //                 string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //                     password: usuario.Clave,
        //                     salt: Encoding.ASCII.GetBytes(configuration["Salt"]),
        //                     prf: KeyDerivationPrf.HMACSHA1,
        //                     iterationCount: 1000,
        //                     numBytesRequested: 256 / 8));

        //                 repositorio.ModificacionClave(id, hashed);
        //             }
        //             else
        //             {
        //                 usuario.Clave = usuarioExistente.Clave;
        //                 repositorio.Modificacion(usuario);
        //             }

        //             return RedirectToAction(vista);
        //         }

        //         //  Si es admin  
        //         if (string.IsNullOrEmpty(usuario.Clave))
        //         {
        //             usuario.Clave = usuarioExistente.Clave;

        //             if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
        //             {
        //                 GuardarAvatar(usuario);
        //             }
        //             else
        //             {
        //                 usuario.Avatar = usuarioExistente.Avatar;
        //             }

        //             repositorio.Modificacion(usuario);
        //             ViewBag.Roles = Usuario.ObtenerRoles();
        //             return Redirect(returnUrl);
        //         }
        //         else // Si se está cambiando la clave
        //         {
        //             string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //                 password: usuario.Clave,
        //                 salt: Encoding.ASCII.GetBytes(configuration["Salt"]),
        //                 prf: KeyDerivationPrf.HMACSHA1,
        //                 iterationCount: 1000,
        //                 numBytesRequested: 256 / 8));

        //             repositorio.ModificacionClave(id, hashed);
        //             return Redirect(returnUrl);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //         return View(usuario);
        //     }
        // }

        // public ActionResult Edit(int id, Usuario usuario)
        // {
        //     var vista = ""; // Nombre de la vista de retorno
        //     var returnUrl = TempData["returnUrl"]?.ToString() ?? "/";

        //     try
        //     {
        //         // Obtener usuario actual desde la DB
        //         var usuarioExistente = repositorio.ObtenerPorId(id);

        //         // Si el usuario no es administrador
        //         if (!User.IsInRole("Administrador"))
        //         {
        //             vista = nameof(Perfil);
        //             if (usuario.Clave == null)
        //             {
        //                 usuario.Clave = usuarioExistente.Clave;
        //             }
        //             usuario.Id = usuarioExistente.Id;
        //             usuario.Rol = usuarioExistente.Rol;

        //             // Avatar: si no se seleccionó uno nuevo, mantener el existente
        //             if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
        //             {
        //                 GuardarAvatar(usuario);
        //             }
        //             else
        //             {
        //                 usuario.Avatar = usuarioExistente.Avatar;
        //             }

        //             repositorio.Modificacion(usuario);
        //             ViewBag.Roles = Usuario.ObtenerRoles();
        //             return RedirectToAction(vista);
        //         }

        //         // Si es admin y no se está cambiando la clave
        //         if (usuario.Clave == null)
        //         {
        //             usuario.Clave = usuarioExistente.Clave;

        //             if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
        //             {
        //                 GuardarAvatar(usuario);
        //             }
        //             else
        //             {
        //                 usuario.Avatar = usuarioExistente.Avatar;
        //             }

        //             repositorio.Modificacion(usuario);
        //             ViewBag.Roles = Usuario.ObtenerRoles();
        //             return Redirect(returnUrl);
        //         }
        //         else // Si se está cambiando la clave
        //         {
        //             string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //                 password: usuario.Clave,
        //                 salt: Encoding.ASCII.GetBytes(configuration["Salt"]),
        //                 prf: KeyDerivationPrf.HMACSHA1,
        //                 iterationCount: 1000,
        //                 numBytesRequested: 256 / 8));

        //             repositorio.ModificacionClave(id, hashed);
        //             return Redirect(returnUrl);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //         return View(usuario);
        //     }
        // }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<ActionResult> Edit(int id, Usuario usuario)
        // {
        //     var returnUrl = TempData["returnUrl"]?.ToString() ?? "/";
        //     try
        //     {
        //         var usuarioExistente = repositorio.ObtenerPorId(id);

        //         if (usuarioExistente == null)
        //             return NotFound();

        //         // --- Si no es admin, solo puede editar su propio perfil ---
        //         if (!User.IsInRole("Administrador"))
        //         {
        //             // Fuerzo valores que no puede cambiar
        //             usuario.Id = usuarioExistente.Id;
        //             usuario.Rol = usuarioExistente.Rol;

        //             // Si no cambia la clave, mantengo la existente
        //             if (string.IsNullOrEmpty(usuario.Clave))
        //             {
        //                 usuario.Clave = usuarioExistente.Clave;
        //             }
        //             else
        //             {
        //                 // Re-hash de nueva clave
        //                 string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //                     password: usuario.Clave,
        //                     salt: Encoding.ASCII.GetBytes(configuration["Salt"]),
        //                     prf: KeyDerivationPrf.HMACSHA1,
        //                     iterationCount: 1000,
        //                     numBytesRequested: 256 / 8));
        //                 usuario.Clave = hashed;
        //             }

        //             // Avatar
        //             if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
        //             {
        //                 GuardarAvatar(usuario);
        //             }
        //             else
        //             {
        //                 usuario.Avatar = usuarioExistente.Avatar;
        //             }

        //             repositorio.Modificacion(usuario);

        //             //  Si es el usuario logueado, refrescar claims
        //             await RefrescarClaimsSiEsActual(usuario);

        //             return RedirectToAction(nameof(Perfil));
        //         }

        //         // --- Si es administrador ---
        //         if (string.IsNullOrEmpty(usuario.Clave))
        //         {
        //             usuario.Clave = usuarioExistente.Clave;
        //         }
        //         else
        //         {
        //             string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //                 password: usuario.Clave,
        //                 salt: Encoding.ASCII.GetBytes(configuration["Salt"]),
        //                 prf: KeyDerivationPrf.HMACSHA1,
        //                 iterationCount: 1000,
        //                 numBytesRequested: 256 / 8));
        //             usuario.Clave = hashed;
        //         }

        //         // Avatar
        //         if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
        //         {
        //             GuardarAvatar(usuario);
        //         }
        //         else
        //         {
        //             usuario.Avatar = usuarioExistente.Avatar;
        //         }

        //         repositorio.Modificacion(usuario);

        //         // Si es el usuario logueado, refrescar claims
        //         await RefrescarClaimsSiEsActual(usuario);

        //         return Redirect(returnUrl);
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex);
        //         return View(usuario);
        //     }
        // }
        // private async Task RefrescarClaimsSiEsActual(Usuario usuario)
        // {
        //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //     if (userId == usuario.Id.ToString())
        //     {
        //         var claims = new List<Claim>
        // {
        //     new Claim(ClaimTypes.Name, usuario.Email),
        //     new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
        //     new Claim(ClaimTypes.Role, usuario.RolNombre),
        //     new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
        // };

        //         var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //         var principal = new ClaimsPrincipal(identity);

        //         await HttpContext.SignInAsync(
        //             CookieAuthenticationDefaults.AuthenticationScheme,
        //             principal,
        //             new AuthenticationProperties
        //             {
        //                 IsPersistent = true,
        //                 ExpiresUtc = DateTime.UtcNow.AddHours(1)
        //             });
        //     }
        // }



        // // Método auxiliar para guardar avatar
        // private void GuardarAvatar(Usuario usuario)
        // {
        //     string wwwPath = environment.WebRootPath;
        //     string path = Path.Combine(wwwPath, "Uploads");
        //     if (!Directory.Exists(path))
        //         Directory.CreateDirectory(path);

        //     string fileName = "avatar_" + usuario.Id + Path.GetExtension(usuario.AvatarFile.FileName);
        //     string pathCompleto = Path.Combine(path, fileName);

        //     using (var stream = new FileStream(pathCompleto, FileMode.Create))
        //     {
        //         usuario.AvatarFile.CopyTo(stream);
        //     }

        //     usuario.Avatar = Path.Combine("/Uploads", fileName);
        // }

  // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit(int id, Usuario usuario)
        {
            var returnUrl = TempData["returnUrl"]?.ToString() ?? "/";
            try
            {
                var usuarioExistente = repositorio.ObtenerPorId(id);
                if (usuarioExistente == null)
                    return NotFound();

                // --- Solo permitir que un usuario normal edite su perfil ---
                if (!User.IsInRole("Administrador"))
                {
                    usuario.Id = usuarioExistente.Id;
                    usuario.Rol = usuarioExistente.Rol; // No puede cambiar rol

                    // Contraseña opcional
                    if (!string.IsNullOrEmpty(usuario.Clave))
                    {
                        usuario.Clave = HashPassword(usuario.Clave);
                    }
                    else
                    {
                        usuario.Clave = usuarioExistente.Clave;
                    }

                    // Avatar
                    if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
                    {
                        GuardarAvatar(usuario);
                    }
                    else
                    {
                        usuario.Avatar = usuarioExistente.Avatar;
                    }

                    repositorio.Modificacion(usuario);

                    // Refrescar claims si es el usuario logueado
                    await RefrescarClaimsSiEsActual(usuario);

                    return RedirectToAction(nameof(Perfil));
                }

                // --- Administrador ---
                usuario.Rol = usuario.Rol; // puede cambiar rol

                // Contraseña opcional
                if (!string.IsNullOrEmpty(usuario.Clave))
                {
                    usuario.Clave = HashPassword(usuario.Clave);
                }
                else
                {
                    usuario.Clave = usuarioExistente.Clave;
                }

                // Avatar
                if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
                {
                    GuardarAvatar(usuario);
                }
                else
                {
                    usuario.Avatar = usuarioExistente.Avatar;
                }

                repositorio.Modificacion(usuario);

                // Refrescar claims si es el usuario logueado
                await RefrescarClaimsSiEsActual(usuario);

                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View(usuario);
            }
        }

// Método para hash de contraseña
private string HashPassword(string password)
{
    return Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password,
        salt: Encoding.ASCII.GetBytes(configuration["Salt"]),
        prf: KeyDerivationPrf.HMACSHA1,
        iterationCount: 1000,
        numBytesRequested: 256 / 8));
}

// Refrescar claims del usuario logueado
private async Task RefrescarClaimsSiEsActual(Usuario usuario)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == usuario.Id.ToString())
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Email),
            new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
            new Claim(ClaimTypes.Role, usuario.RolNombre),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(1)
            });
    }
}

// Guardar avatar
private void GuardarAvatar(Usuario usuario)
{
    string wwwPath = environment.WebRootPath;
    string uploadsPath = Path.Combine(wwwPath, "Uploads");
    if (!Directory.Exists(uploadsPath))
        Directory.CreateDirectory(uploadsPath);

    string fileName = "avatar_" + usuario.Id + Path.GetExtension(usuario.AvatarFile.FileName);
    string filePath = Path.Combine(uploadsPath, fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        usuario.AvatarFile.CopyTo(stream);
    }

    // ⚡ Ruta web correcta con /
    usuario.Avatar = "/Uploads/" + fileName;
}

        // GET: Usuarios/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            Usuario usuario = repositorio.ObtenerPorId(id);
            return View(usuario);

        }

        // POST: Usuarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Usuario usuario)
        {
            try
            {
                repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(usuario);
            }
        }

        [AllowAnonymous]
        // GET: Usuarios/Login/
        public ActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        // POST: Usuarios/Login/
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            try
            {
                var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"].ToString();
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: login.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    var e = repositorio.ObtenerPorEmail(login.Usuario);
                    if (e == null || e.Clave != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos");
                        TempData["returnUrl"] = returnUrl;
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                        new Claim(ClaimTypes.Role, e.RolNombre),
                        new Claim(ClaimTypes.NameIdentifier, e.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));
                    TempData.Remove("returnUrl");
                    return Redirect(returnUrl);
                }
                TempData["returnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }


        // GET: /salir
        [Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        // ...

        [HttpPost("api/login")]
        [AllowAnonymous]
        public IActionResult ApiLogin([FromBody] LoginViewModel login)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: login.Clave,
                    salt: Encoding.ASCII.GetBytes(configuration["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                var e = repositorio.ObtenerPorEmail(login.Usuario);
                if (e == null || e.Clave != hashed)
                {
                    return Unauthorized(new { message = "El email o la clave no son correctos" });
                }

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, e.Email),
            new Claim("FullName", e.Nombre + " " + e.Apellido),
            new Claim(ClaimTypes.Role, e.RolNombre),
        };

                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: configuration["TokenAuthentication:Issuer"],
                    audience: configuration["TokenAuthentication:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: creds
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}