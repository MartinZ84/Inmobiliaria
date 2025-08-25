using MySql.Data.MySqlClient;
using Inmobiliaria.Models.Entidades;
using System.Data;

namespace Inmobiliaria.Models.Repositorio;

public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
{
	// string connectionString="Server=localhost;User=root;Password=;Database=inmozanche;SslMode=none";


	public RepositorioPropietario(IConfiguration configuration, ILogger<RepositorioPropietario> logger)
	  : base(configuration)
	{
		_logger = logger;
	}

	public RepositorioPropietario(IConfiguration configuration) : base(configuration)
	{
	}

	private readonly ILogger<RepositorioPropietario> _logger;

	public IList<Propietario?> ObtenerTodos()
	{
		IList<Propietario?> res = new List<Propietario?>();
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email" +
				$" FROM Propietarios";
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				// command.CommandType = CommandType.Text;
				connection.Open();
				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					var p = new Propietario
					{
						Id = reader.GetInt32(nameof(Propietario.Id)),
						Nombre = reader.GetString(nameof(Propietario.Nombre)),
						Apellido = reader.GetString(nameof(Propietario.Apellido)),
						Dni = reader.GetString(nameof(Propietario.Dni)),
						Telefono = reader.GetString(nameof(Propietario.Telefono)),
						Email = reader.GetString(nameof(Propietario.Email)),

					};
					res.Add(p);
				}
				connection.Close();
			}
		}
		return res;
	}

	

	public int Alta(Propietario p)
	{
		int res = -1;


		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = $"INSERT INTO Propietarios (Nombre, Apellido, Dni, Telefono, Email) " +
				$"VALUES (@{nameof(p.Nombre)}, @{nameof(p.Apellido)}, @{nameof(p.Dni)}, @{nameof(p.Telefono)}, @{nameof(p.Email)});" +
				"SELECT LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{

				command.Parameters.AddWithValue($"@{nameof(p.Nombre)}", p.Nombre);
				command.Parameters.AddWithValue($"@{nameof(p.Apellido)}", p.Apellido);
				command.Parameters.AddWithValue($"@{nameof(p.Dni)}", p.Dni);
				command.Parameters.AddWithValue($"@{nameof(p.Telefono)}", p.Telefono);
				command.Parameters.AddWithValue($"@{nameof(p.Email)}", p.Email);

				connection.Open();
				res = Convert.ToInt32(command.ExecuteScalar());
				p.Id = res;
				connection.Close();
			}
		}
		return res;
	}

	public int Modificacion(Propietario p)
	{
		int res = -1;
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = $"UPDATE Propietarios SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email " +
				$"WHERE Id = @id";

			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				command.Parameters.AddWithValue($"@{nameof(p.Id)}", p.Id);
				command.Parameters.AddWithValue($"@{nameof(p.Nombre)}", p.Nombre);
				command.Parameters.AddWithValue($"@{nameof(p.Apellido)}", p.Apellido);
				command.Parameters.AddWithValue($"@{nameof(p.Dni)}", p.Dni);
				command.Parameters.AddWithValue($"@{nameof(p.Telefono)}", p.Telefono);
				command.Parameters.AddWithValue($"@{nameof(p.Email)}", p.Email);

				connection.Open();
				res = command.ExecuteNonQuery();
				connection.Close();
			}
		}
		return res;
	}


	public Propietario? ObtenerPorId(int id)
	{
		Propietario? p = null;
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email" +
				$" FROM Propietarios" + $" WHERE id=@id";
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				// command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue($"@{nameof(id)}", id);
				connection.Open();
				var reader = command.ExecuteReader();
				if (reader.Read())
				{
					p = new Propietario
					{
						Id = reader.GetInt32(nameof(Propietario.Id)),
						Nombre = reader.GetString(nameof(Propietario.Nombre)),
						Apellido = reader.GetString(nameof(Propietario.Apellido)),
						Dni = reader.GetString(nameof(Propietario.Dni)),
						Telefono = reader.GetString(nameof(Propietario.Telefono)),
						Email = reader.GetString(nameof(Propietario.Email)),

					};
				}
				connection.Close();
			}
		}
		return p;
	}

	public Propietario? ObtenerPorNombre(string nombre)
	{
		Propietario? p = null;
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email" +
				$" FROM Propietarios" + $" WHERE Nombre=@nombre";
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				// command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue($"@{nameof(nombre)}", nombre);
				connection.Open();
				var reader = command.ExecuteReader();
				if (reader.Read())
				{
					p = new Propietario
					{
						Id = reader.GetInt32(nameof(Propietario.Id)),
						Nombre = reader.GetString(nameof(Propietario.Nombre)),
						Apellido = reader.GetString(nameof(Propietario.Apellido)),
						Dni = reader.GetString(nameof(Propietario.Dni)),
						Telefono = reader.GetString(nameof(Propietario.Telefono)),
						Email = reader.GetString(nameof(Propietario.Email)),
					};
				}
				connection.Close();
			}
		}
		return p;
	}

	public int Baja(int id)
	{
		int res = -1;
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = $"DELETE FROM Propietarios WHERE Id = @id";
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				// command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@id", id);
				connection.Open();
				res = command.ExecuteNonQuery();
				connection.Close();
			}
		}
		return res;
	}

	public IList<Propietario> ObtenerLista(int paginaNro = 1, int tamPagina = 5)
	{
		IList<Propietario> res = new List<Propietario>();
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = @$"
					SELECT Id, Nombre, Apellido, Dni, Telefono, Email
					FROM Propietarios
					LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
				";
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				command.CommandType = CommandType.Text;
				connection.Open();
				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					Propietario p = new Propietario
					{
						Id = reader.GetInt32(nameof(Propietario.Id)),//más seguro
						Nombre = reader.GetString(nameof(Propietario.Nombre)),
						Apellido = reader.GetString("Apellido"),
						Dni = reader.GetString("Dni"),
						Telefono = reader.GetString("Telefono"),
						Email = reader.GetString("Email"),

					};
					res.Add(p);
				}
				connection.Close();
			}
		}
		return res;
	}

	public int ObtenerCantidad()
	{
		int res = 0;
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = @$"
					SELECT COUNT(Id)
					FROM Propietarios
				";
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				command.CommandType = CommandType.Text;
				connection.Open();
				var reader = command.ExecuteReader();
				if (reader.Read())
				{
					res = reader.GetInt32(0);
				}
				connection.Close();
			}
		}
		return res;
	}

	public IList<Propietario> BuscarPorNombre(string nombre)
	{
		List<Propietario> res = new List<Propietario>();
		Propietario p = null;
		nombre = "%" + nombre + "%";
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = @"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Clave 
					FROM Propietarios
					WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				command.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = nombre;
				command.CommandType = CommandType.Text;
				connection.Open();
				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					p = new Propietario
					{
						Id = reader.GetInt32(nameof(Propietario.Id)),
						Nombre = reader.GetString("Nombre"),
						Apellido = reader.GetString("Apellido"),
						Dni = reader.GetString("Dni"),
						Telefono = reader.GetString("Telefono"),
						Email = reader.GetString("Email")

					};
					res.Add(p);
				}
				connection.Close();
			}
		}
		return res;
	}

	public Propietario ObtenerPorEmail(string email)
	{
		Propietario p = null;
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = @"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Clave 
					FROM Propietarios
					WHERE Email=@email";
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				command.CommandType = CommandType.Text;
				command.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
				connection.Open();
				var reader = command.ExecuteReader();
				if (reader.Read())
				{
					p = new Propietario
					{
						Id = reader.GetInt32(nameof(Propietario.Id)),
						Nombre = reader.GetString("Nombre"),
						Apellido = reader.GetString("Apellido"),
						Dni = reader.GetString("Dni"),
						Telefono = reader.GetString("Telefono"),
						Email = reader.GetString("Email")

					};
				}
				connection.Close();
			}
		}
		return p;
	}

	public string? ExisteDniPropietario(string dni)
	{
		bool existe = false;
		try
		{
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "SELECT EXISTS(SELECT 1 FROM Propietarios WHERE DNI = @dni)";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@dni", dni);
					connection.Open();
					var result = command.ExecuteScalar();
					existe = result != null && Convert.ToBoolean(result);
					connection.Close();
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError($"Error al verificar DNI: {dni}", ex);
			// No lanza excepción, retorna false por defecto
			// Esto significa "no existe" en caso de error
			return $"Error al verificar DNI: {dni}";
		}
		return existe ? $" El DNI {dni} ya existente en otro propietario" : null;
	}
	
	
public List<Propietario> BuscarPropietariosConValidacion(string? dni = null, string? nombre = null, 
    string? apellido = null, string? email = null)
{
    // Validar que al menos un criterio esté presente
    bool tieneCriterios = !string.IsNullOrWhiteSpace(dni) || 
                         !string.IsNullOrWhiteSpace(nombre) || 
                         !string.IsNullOrWhiteSpace(apellido) || 
                         !string.IsNullOrWhiteSpace(email);

    if (!tieneCriterios)
    {
        return new List<Propietario>(); // Retorna lista vacía si no hay criterios
    }

    List<Propietario> propietarios = new List<Propietario>();
    
    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        string sql = @"SELECT Id, nombre, apellido, dni, telefono, email 
                      FROM Propietarios 
                      WHERE (@dni IS NULL OR Dni LIKE CONCAT('%', @dni, '%'))
                        AND (@nombre IS NULL OR Nombre LIKE CONCAT('%', @nombre, '%'))
                        AND (@apellido IS NULL OR Apellido LIKE CONCAT('%', @apellido, '%'))
                        AND (@email IS NULL OR Email LIKE CONCAT('%', @email, '%'))
                      ORDER BY Apellido, Nombre
                      LIMIT 200"; // Límite fijo para evitar sobrecarga

        using (MySqlCommand command = new MySqlCommand(sql, connection))
        {
            command.CommandType = CommandType.Text;
            
            command.Parameters.Add("@dni", MySqlDbType.VarChar).Value = 
                string.IsNullOrWhiteSpace(dni) ? DBNull.Value : dni.Trim();
            command.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = 
                string.IsNullOrWhiteSpace(nombre) ? DBNull.Value : nombre.Trim();
            command.Parameters.Add("@apellido", MySqlDbType.VarChar).Value = 
                string.IsNullOrWhiteSpace(apellido) ? DBNull.Value : apellido.Trim();
            command.Parameters.Add("@email", MySqlDbType.VarChar).Value = 
                string.IsNullOrWhiteSpace(email) ? DBNull.Value : email.Trim();
            
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    propietarios.Add(new Propietario
					{					
						Id = reader.GetInt32(nameof(Propietario.Id)),
						Nombre = reader.GetString(nameof(Propietario.Nombre)),
						Apellido = reader.GetString(nameof(Propietario.Apellido)),
						Dni = reader.GetString(nameof(Propietario.Dni)),
						Telefono = reader.GetString(nameof(Propietario.Telefono)),
						Email = reader.GetString(nameof(Propietario.Email)),
                    });
                }
            }
        }
    }
    
    return propietarios;
}
}