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
        public ActionResult Index(string? nombre = null, string? apellido = null, string? email = null, int pagina = 1)
        {
            try
            {
                IList<Usuario> Usuarios;
                var tamaño = 6;

                if (!string.IsNullOrWhiteSpace(nombre) || !string.IsNullOrWhiteSpace(apellido) || !string.IsNullOrWhiteSpace(email))
                {
                    Usuarios = repositorio.BuscarUsuariosConValidacion(nombre, apellido, email, pagina, tamaño);
                    var total = Usuarios.Count;
                    ViewBag.Pagina = pagina;
                    ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
                }
                else
                {
                    Usuarios = repositorio.ObtenerLista(pagina, tamaño);
                    ViewBag.Pagina = pagina;
                    var total = repositorio.ObtenerCantidad();
                    ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
                }
        
                ViewBag.nombre = nombre;
                ViewBag.apellido = apellido;
                ViewBag.email = email;

                return View(Usuarios);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los Usuarios.";
                TempData["AlertType"] = "danger";
                return View(new List<Usuario>());
            }
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
                    TempData["SuccessMessage"] = "Perfil actualizado exitosamente";
                    TempData["AlertType"] = "success";
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
                TempData["SuccessMessage"] = "Usuario editado exitosamente";
                TempData["AlertType"] = "success";

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