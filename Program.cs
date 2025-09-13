// using System.Globalization;
// using Inmobiliaria.Models.Repositorio;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Localization;
// using Microsoft.IdentityModel.Tokens;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddControllersWithViews();
// builder.Services.AddLogging();
// builder.Services.AddScoped<RepositorioPropietario>();
// builder.Services.AddScoped<RepositorioUsuario>();
// builder.Services.AddScoped<RepositorioInquilino>();
// builder.Services.AddScoped<RepositorioInmueble>();
// builder.Services.AddScoped<RepositorioTipoInmueble>();
// builder.Services.AddScoped<RepositorioContrato>();
// builder.Services.AddScoped<RepositorioPago>();


// // 2. Configurar localización ANTES de Build()
// builder.Services.Configure<RequestLocalizationOptions>(options =>
// {
//     var supportedCultures = new[] { "es-ES", "es-MX", "es" };
//     options.DefaultRequestCulture = new RequestCulture("es-ES");
//     options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
//     options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
// });


// //Agregar servicios de autoriazación 
// builder.Services.AddAuthorization(options =>
// {
  
//     // options.AddPolicy("Administrador", policy => policy.RequireClaim("Administrador"));
//     // options.AddPolicy("Usuario", policy => policy.RequireClaim("Usuario"));
//     // options.AddPolicy("Empleado", policy => policy.RequireClaim("Empleado"));
//     //agregar políticas de autorización en empleados para que deje tambien a administrador y superadministrador
//      options.AddPolicy("Empleado", policy => policy.RequireRole("Empleado", "Administrador", "SuperAdministrador"));
//     options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador", "SuperAdministrador"));
// });       

// //Agrega de autenticación con cookie  
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie(options =>
//     {
//         options.LoginPath = "/Usuarios/Login";
//         options.LogoutPath = "/Home/Logout";
//         options.AccessDeniedPath = "/Home/Restringido";
//     })
//     //Agrego autenticacion con token
//     .AddJwtBearer(options =>//la api web valida con token
// 				{
// 					options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
// 					{
// 						ValidateIssuer = true,
// 						ValidateAudience = true,
// 						ValidateLifetime = true,
// 						ValidateIssuerSigningKey = true,
// 						ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],
// 						ValidAudience = builder.Configuration["TokenAuthentication:Audience"],
// 						IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
// 						builder.Configuration["TokenAuthentication:SecretKey"])),
// 					};
// 					// opción extra para usar el token el hub. Que es esto?
// 					// options.Events = new JwtBearerEvents
// 					// {
// 					// 	OnMessageReceived = context =>
// 					// 	{
// 					// 		// Read the token out of the query string
// 					// 		var accessToken = context.Request.Query["access_token"];
// 					// 		// If the request is for our hub...
// 					// 		var path = context.HttpContext.Request.Path;
// 					// 		if (!string.IsNullOrEmpty(accessToken) &&
// 					// 			path.StartsWithSegments("/chatsegurohub"))
// 					// 		{//reemplazar la url por la usada en la ruta ⬆
// 					// 			context.Token = accessToken;
// 					// 		}
// 					// 		return Task.CompletedTask;
// 					// 	}
// 					// };
// 				});;


// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

// app.UseHttpsRedirection();

// // Archivos estáticos: wwwroot
// app.UseStaticFiles();

// app.UseRouting();

// app.UseAuthorization();

// app.MapStaticAssets();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}")
//     .WithStaticAssets();


// app.Run();

using System.Globalization;
using Inmobiliaria.Models.Repositorio;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

// Repositorios
builder.Services.AddScoped<RepositorioPropietario>();
builder.Services.AddScoped<RepositorioUsuario>();
builder.Services.AddScoped<RepositorioInquilino>();
builder.Services.AddScoped<RepositorioInmueble>();
builder.Services.AddScoped<RepositorioTipoInmueble>();
builder.Services.AddScoped<RepositorioContrato>();
builder.Services.AddScoped<RepositorioPago>();

// Localización
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "es-ES", "es-MX", "es" };
    options.DefaultRequestCulture = new RequestCulture("es-ES");
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
});

// Autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Empleado", policy => policy.RequireRole("Empleado", "Administrador", "SuperAdministrador"));
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador", "SuperAdministrador"));
});

// Autenticación: Cookies + JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Usuarios/Login";
    options.LogoutPath = "/Home/Logout";
    options.AccessDeniedPath = "/Home/Restringido";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],
        ValidAudience = builder.Configuration["TokenAuthentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            builder.Configuration["TokenAuthentication:SecretKey"]))
    };
});

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// localización
app.UseRequestLocalization();

// Autenticación primero, después autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
