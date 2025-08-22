#  Inmobiliaria

Sistema web de gestión inmobiliaria desarrollado en **.NET 9** usando el patrón **MVC** con vistas **Razor**, integración con **MySQL** y componentes en **JavaScript**.

##  Descripción

Este proyecto permite gestionar propiedades inmobiliarias —crear, editar, listar y eliminar inmuebles— en un entorno web intuitivo. Está pensado para agencias inmobiliarias o desarrolladores que quieran aprender o implementar un sistema similar.

##  Características principales

- Arquitectura basada en **MVC (.NET 9)**
- Interfaz web con **vistas Razor**
- Lógica dinámica con **JavaScript** (frontend)
- Persistencia de datos usando base de datos **MySQL**
- Configuración diferenciada para entorno de desarrollo (`appsettings.Development.json`) y producción (`appsettings.json`)
- Proyecto estructurado con capas: **Controllers**, **Models**, **Views**, **Data**, **Properties**

##  Estructura del proyecto

Inmobiliaria/
├── Controllers/ # Controladores MVC
├── Data/ # Contexto y configuraciones de base de datos
├── Models/ # Clases de modelo (entidades del dominio)
├── Properties/ # Archivos de propiedades del proyecto
├── Views/ # Vistas Razor (.cshtml)
├── wwwroot/ # Archivos estáticos (JS, CSS, imágenes)
├── appsettings.json # Configuración de producción
├── appsettings.Development.json # Configuración para desarrollo
├── Inmobiliaria.csproj # Proyecto .NET
├── Inmobiliaria.sln # Solución .NET
├── Program.cs # Punto de entrada y configuración del host
├── .gitignore # Archivos ignorados por Git
└── README.md # Esta documentación

##  Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- Servidor MySQL accesible (puede ser local o remoto)
- (Opcional) IDE como **Visual Studio 2022+** o **Visual Studio Code**

##  Instalación y configuración

1. Clona el repositorio:
    ```bash
    git clone https://github.com/MartinZ84/Inmobiliaria.git
    cd Inmobiliaria
    ```

2. Configura la cadena de conexión en `appsettings.json` (o `appsettings.Development.json` para entorno de desarrollo):

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=TU_SERVIDOR;Database=InmobiliariaDb;User=TU_USUARIO;Password=TU_CONTRASEÑA;"
    }
    ```

3. (Opcional) Habilitá migraciones de Entity Framework Core si el proyecto las usa:

    ```bash
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```

4. Ejecutá el proyecto:
    ```bash
    dotnet run
    ```

5. Abrí tu navegador en `https://localhost:5001` o donde indique la consola.

##  Uso

Una vez en funcionamiento, podés:

- Navegar a la interfaz web para listar, crear, editar o eliminar propiedades.
- Revisar los controladores en la carpeta `Controllers`, los modelos en `Models`, y las vistas Razor en `Views`.
- Personalizar la presentación con CSS o JS en `wwwroot`.

##  Dependencias

- ASP.NET Core MVC
- Entity Framework Core (si está incluido en `Data/`)
- MySQL connector para .NET
- Librerías JS (según lo que uses en `wwwroot`, por ejemplo jQuery)

##  Contribuciones

Las contribuciones son bienvenidas. Para colaborar:

1. Forkeá este repositorio.
2. Creá una rama para tu feature o reparación (`git checkout -b feature/nombre-funcionalidad`).
3. Hacé commits con buenos mensajes descriptivos.
4. Mandá un Pull Request explicando los cambios.

##  Licencia

Este proyecto está bajo la licencia **MIT**. Mirá el archivo [LICENSE](LICENSE) para más detalles.
