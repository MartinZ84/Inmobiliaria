using MySql.Data.MySqlClient;
using Inmobiliaria.Models.Entidades;
using Microsoft.Extensions.Logging;

namespace Inmobiliaria.Models.Repositorio;

public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
{
	// string ConnectionString="Server=localhost;User=root;Password=;Database=inmozanche;SslMode=none";
	// public RepositorioInquilino()
	// {

	// }
	public RepositorioInquilino(IConfiguration configuration, ILogger<RepositorioInquilino> logger)
		: base(configuration)
	{
		_logger = logger;
	}


	private readonly ILogger<RepositorioInquilino> _logger;


	public IList<Inquilino?> ObtenerTodos()
	{
		IList<Inquilino?> res = new List<Inquilino?>();
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email " +
				// , Lugar_trabajo , Dni_garante,Nombre_garante,Apellido_garante,Telefono_garante" +
				$" FROM Inquilinos";
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				// command.CommandType = CommandType.Text;
				connection.Open();
				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					var i = new Inquilino
					{
						Id = reader.GetInt32(nameof(Inquilino.Id)),
						Nombre = reader.GetString(nameof(Inquilino.Nombre)),
						Apellido = reader.GetString(nameof(Inquilino.Apellido)),
						Dni = reader.GetString(nameof(Inquilino.Dni)),
						Telefono = reader.GetString(nameof(Inquilino.Telefono)),
						Email = reader.GetString(nameof(Inquilino.Email))

					};
					res.Add(i);
				}
				connection.Close();
			}
		}
		return res;
	}

	public int Alta(Inquilino i)
	{
		int res = -1;
	
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{

			string sql = $"INSERT INTO Inquilinos (Nombre, Apellido, Dni, Telefono, Email ) " +
				// , Lugar_Trabajo,Dni_Garante, Nombre_Garante, Apellido_Garante, Telefono_Garante) " +
				$"VALUES (@{nameof(i.Nombre)}, @{nameof(i.Apellido)}, @{nameof(i.Dni)}, @{nameof(i.Telefono)}, @{nameof(i.Email)} );" +
				// @{nameof(i.Dni_Garante)}, @{nameof(i.Nombre_Garante)} ,@{nameof(i.Apellido_Garante)}, @{nameof(i.Telefono_Garante)} );" +
				"SELECT LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{

				command.Parameters.AddWithValue($"@{nameof(i.Nombre)}", i.Nombre);
				command.Parameters.AddWithValue($"@{nameof(i.Apellido)}", i.Apellido);
				command.Parameters.AddWithValue($"@{nameof(i.Dni)}", i.Dni);
				command.Parameters.AddWithValue($"@{nameof(i.Telefono)}", i.Telefono);
				command.Parameters.AddWithValue($"@{nameof(i.Email)}", i.Email);

				connection.Open();
				res = Convert.ToInt32(command.ExecuteScalar());
				i.Id = res;
				connection.Close();
			}
		}
		return res;
	}

	public int Modificacion(Inquilino i)
	{
		int res = -1;
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = $"UPDATE Inquilinos SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email " +
				// , Lugar_Trabajo=@lugar_trabajo ,Dni_Garante=@dni_garante, Nombre_Garante=@nombre_garante, Apellido_Garante=@apellido_garante, Telefono_Garante=@telefono_garante " +
				$"WHERE Id = @id";

			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				command.Parameters.AddWithValue($"@{nameof(i.Id)}", i.Id);
				command.Parameters.AddWithValue($"@{nameof(i.Nombre)}", i.Nombre);
				command.Parameters.AddWithValue($"@{nameof(i.Apellido)}", i.Apellido);
				command.Parameters.AddWithValue($"@{nameof(i.Dni)}", i.Dni);
				command.Parameters.AddWithValue($"@{nameof(i.Telefono)}", i.Telefono);
				command.Parameters.AddWithValue($"@{nameof(i.Email)}", i.Email);


				connection.Open();
				res = command.ExecuteNonQuery();
				connection.Close();
			}
		}
		return res;
	}


	public Inquilino? ObtenerPorId(int id)
	{
		Inquilino? i = null;
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email " +
				// , Lugar_trabajo, Dni_garante,Nombre_garante,Apellido_garante,Telefono_garante " +
				$" FROM Inquilinos" + $" WHERE id=@id";
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				// command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue($"@{nameof(id)}", id);
				connection.Open();
				var reader = command.ExecuteReader();
				if (reader.Read())
				{
					i = new Inquilino
					{
						Id = reader.GetInt32(nameof(Inquilino.Id)),
						Nombre = reader.GetString(nameof(Inquilino.Nombre)),
						Apellido = reader.GetString(nameof(Inquilino.Apellido)),
						Dni = reader.GetString(nameof(Inquilino.Dni)),
						Telefono = reader.GetString(nameof(Inquilino.Telefono)),
						Email = reader.GetString(nameof(Inquilino.Email)),
					};
				}
				connection.Close();
			}
		}
		return i;
	}

	public int Baja(int id)
	{
		int res = -1;
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = $"DELETE FROM Inquilinos WHERE Id = @id";
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


	public string? ExisteDniInquilino(string dni)
	{
		bool existe = false;
		try
		{
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "SELECT EXISTS(SELECT 1 FROM Inquilinos WHERE DNI = @dni)";
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
		return existe ? $"El DNI {dni} ya existente en otro inquilino" : null;
	}
}