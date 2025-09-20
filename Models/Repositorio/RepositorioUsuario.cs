using MySql.Data.MySqlClient;
using Inmobiliaria.Models;

namespace Inmobiliaria.Models.Repositorio
{
	public class RepositorioUsuario : RepositorioBase
	{
		public RepositorioUsuario(IConfiguration configuration, ILogger<RepositorioUsuario> logger)
		  : base(configuration)
		{
			_logger = logger;
		}

		public RepositorioUsuario(IConfiguration configuration) : base(configuration)
		{
		}

		private readonly ILogger<RepositorioUsuario> _logger;

		public IList<Usuario> ObtenerTodos()
		{
			IList<Usuario> res = new List<Usuario>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol" +
					$" FROM Usuarios";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					// command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Usuario e = new Usuario
						{
							Id = reader.GetInt32(nameof(Usuario.Id)),
							Nombre = reader.GetString(nameof(Usuario.Nombre)),
							Apellido = reader.GetString(nameof(Usuario.Apellido)),
							Avatar = reader[nameof(Usuario.Avatar)].ToString(),
							Email = reader.GetString(nameof(Usuario.Email)),
							Clave = reader.GetString(nameof(Usuario.Clave)),
							Rol = reader.GetInt32(nameof(Usuario.Rol)),
						};
						res.Add(e);
					}
					connection.Close();
				}
			}
			return res;
		}

		public int Alta(Usuario e)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Usuarios (Nombre, Apellido, Avatar, Email, Clave, Rol) " +
					$"VALUES (@nombre, @apellido, @avatar, @email, @clave, @rol);" +
					"SELECT LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{

					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					if (String.IsNullOrEmpty(e.Avatar))
						command.Parameters.AddWithValue("@avatar", DBNull.Value);
					else
						command.Parameters.AddWithValue("@avatar", e.Avatar);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@clave", e.Clave);
					command.Parameters.AddWithValue("@rol", e.Rol);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					e.Id = res;
					connection.Close();
				}
			}
			return res;
		}

		public int Baja(int id)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $"DELETE FROM Usuarios WHERE Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{

					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int Modificacion(Usuario e)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $"UPDATE Usuarios SET Nombre=@nombre, Apellido=@apellido, Avatar=@avatar, Email=@email, Clave=@clave, Rol=@rol " +
					$"WHERE Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					command.Parameters.AddWithValue("@avatar", e.Avatar);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@clave", e.Clave);
					command.Parameters.AddWithValue("@rol", e.Rol);
					command.Parameters.AddWithValue("@id", e.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int ModificacionClave(int id, string claveNueva)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $"UPDATE Usuarios SET Clave=@clave " +
					$"WHERE Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@clave", claveNueva);
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public Usuario ObtenerPorId(int id)
		{
			Usuario? e = null;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol FROM Usuarios" +
					$" WHERE Id=@id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						e = new Usuario
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Avatar = reader["Avatar"].ToString(),
							Email = reader.GetString(4),
							Clave = reader.GetString(5),
							Rol = reader.GetInt32(6),
						};
					}
					connection.Close();
				}
			}
			return e;
		}

		public Usuario ObtenerPorEmail(string email)
		{
			Usuario? e = null;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol FROM Usuarios" +
					$" WHERE Email=@email";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@email", email);

					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						e = new Usuario
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Avatar = reader["Avatar"].ToString(),
							Email = reader.GetString(4),
							Clave = reader.GetString(5),
							Rol = reader.GetInt32(6),
						};
					}
					connection.Close();
				}
			}
			return e;
		}

		public IList<Usuario> BuscarUsuariosConValidacion(string? Nombre = null, string? Apellido = null, string? Email = null, int paginaNro = 1, int tamPagina = 6)
		{
			var Usuarios = new List<Usuario>();

			if (string.IsNullOrWhiteSpace(Nombre) && string.IsNullOrWhiteSpace(Apellido) && string.IsNullOrWhiteSpace(Email))
			{
				return Usuarios;
			}

			using (var connection = new MySqlConnection(connectionString))
			{
				var sql = @"
            SELECT Usuarios.*, Usuarios.nombre, Usuarios.apellido, Usuarios.Email
            FROM Usuarios
			Where 1=1 ";

				var parameters = new List<MySqlParameter>();

				if (!string.IsNullOrWhiteSpace(Nombre))
				{
					sql += " AND Usuarios.nombre LIKE @nombre";
					parameters.Add(new MySqlParameter("@nombre", $"%{Nombre.Trim()}%"));
				}

				if (!string.IsNullOrWhiteSpace(Apellido))
				{
					sql += " AND Usuarios.apellido LIKE @apellido";
					parameters.Add(new MySqlParameter("@apellido", $"%{Apellido.Trim()}%"));
				}

				if (!string.IsNullOrWhiteSpace(Email))
				{
					sql += " AND Usuarios.email LIKE @email";
					parameters.Add(new MySqlParameter("@email", $"%{Email.Trim()}%"));
				}

				sql += $" ORDER BY Usuarios.id LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina} ";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddRange(parameters.ToArray());
					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							Usuarios.Add(MapearUsuarios(reader));
						}
					}
				}
			}
			return Usuarios;
		}

		private Usuario MapearUsuarios(MySqlDataReader reader)
		{
			return new Usuario
			{
				Id = reader.GetInt32("id"),
				Nombre = reader.GetString("nombre"),
				Apellido = reader.GetString("apellido"),
				Avatar = reader.GetString("avatar"),
				Email = reader.GetString("email"),
				Clave = reader.GetString("clave"),
				Rol = reader.GetInt32("rol"),
			};
		}

		public IList<Usuario> ObtenerLista(int paginaNro = 1, int tamPagina = 5)
		{
			IList<Usuario> res = new List<Usuario>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT Usuarios.*, Usuarios.nombre, Usuarios.apellido, Usuarios.email
                    FROM Usuarios 
					LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}        
				";

				using (var command = new MySqlCommand(sql, connection))
				{
					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							res.Add(MapearUsuarios(reader));
						}
					}
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
					FROM Usuarios;
				";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = System.Data.CommandType.Text;
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
  }
 
}