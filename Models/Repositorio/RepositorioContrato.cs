using MySql.Data.MySqlClient;
using Inmobiliaria.Models;
using Inmobiliaria.Models.Entidades;
using System.Data;

namespace Inmobiliaria.Models.Repositorio
{

	public class RepositorioContrato : RepositorioBase
	{
		public RepositorioContrato(IConfiguration configuration, ILogger<RepositorioContrato> logger) : base(configuration)
		{
			_logger = logger;
		}



		public RepositorioContrato(IConfiguration configuration) : base(configuration)
		{
		}

		private readonly ILogger<RepositorioContrato> _logger;




		public IList<Contrato> ObtenerTodos(int paginaNro = 1, int tamPagina = 10)
		{
			IList<Contrato> res = new List<Contrato>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @$"SELECT c.Id, FechaInicio,FechaFin, FechaFinAnt, c.Estado, c.Precio, InquilinoId, InmuebleId, " +
					" inq.Nombre, inq.Apellido, inm.Id, inm.Direccion, c.UsuarioAlta, c.UsuarioBaja " +
					" FROM Contratos c INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id " +
					"INNER JOIN Inmuebles inm ON inm.Id= c.InmuebleId " + "ORDER BY c.FechaInicio ASC " +
					$"LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					//command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato contrato = new Contrato
						{
							Id = reader.GetInt32(nameof(Contrato.Id)),
							FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
							FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
							FechaFinAnt = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.FechaFinAnt)))
								? (DateTime?)null
								: reader.GetDateTime(reader.GetOrdinal(nameof(Contrato.FechaFinAnt))),
							Estado = reader.GetString(nameof(Contrato.Estado)),
							Precio = reader.GetInt32(nameof(Contrato.Precio)),
							InquilinoId = reader.GetInt32(nameof(Contrato.InquilinoId)),
							InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId)),
							Inquilino = new Inquilino
							{
								Id = reader.GetInt32(nameof(Inquilino.Id)),
								Nombre = reader.GetString(nameof(Inquilino.Nombre)),
								Apellido = reader.GetString(nameof(Inquilino.Apellido)),
							},
							Inmueble = new Inmueble
							{
								Id = reader.GetInt32(nameof(Inmueble.Id)),
								Direccion = reader.GetString(nameof(Inmueble.Direccion)),
							},
							UsuarioAlta = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.UsuarioAlta)))
								? (int?)null
								: reader.GetInt32(reader.GetOrdinal(nameof(Contrato.UsuarioAlta))),
							UsuarioBaja = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.UsuarioBaja)))
								? (int?)null
								: reader.GetInt32(reader.GetOrdinal(nameof(Contrato.UsuarioBaja))),
						};
						res.Add(contrato);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Contrato> BuscarContratos(
			string? estado = null,
			DateTime? fechaDesde = null,
			DateTime? fechaHasta = null,
			int? dias = null,
			int paginaNro = 1,
			int tamPagina = 10)
		{
			var contratos = new List<Contrato>();

			using (var connection = new MySqlConnection(connectionString))
			{
				var sql = @$"SELECT c.Id, FechaInicio, FechaFin, FechaFinAnt, c.Estado, c.Precio, InquilinoId, InmuebleId, " +
					" inq.Nombre, inq.Apellido, inm.Id, inm.Direccion, c.UsuarioAlta, c.UsuarioBaja " +
					" FROM Contratos c INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id " +
					"INNER JOIN Inmuebles inm ON inm.Id= c.InmuebleId " +					
					"WHERE 1=1 ";
				var parameters = new List<MySqlParameter>();

				// Filtro por estado
				if (!string.IsNullOrWhiteSpace(estado))
				{
					sql += " AND c.estado = @estado ";
					parameters.Add(new MySqlParameter("@estado", estado.Trim()));
				}

				// // Filtro por fecha de inicio
				// if (fechaDesde.HasValue)
				// {
				// 	sql += " AND c.fechaInicio >= @fechaDesde ";
				// 	parameters.Add(new MySqlParameter("@fechaDesde", fechaDesde.Value));
				// }

				// // Filtro por fecha de fin
				// if (fechaHasta.HasValue)
				// {
				// 	sql += " AND c.fechaFin <= @fechaHasta ";
				// 	parameters.Add(new MySqlParameter("@fechaHasta", fechaHasta.Value));
				// }

				// Filtro por fecha de inicio y fin
				if (fechaDesde.HasValue && fechaHasta.HasValue)
				{
					sql += " AND c.fechaInicio >= @fechaDesde AND c.fechaFinAnt <= @fechaHasta ";
					parameters.Add(new MySqlParameter("@fechaDesde", fechaDesde.Value));
					parameters.Add(new MySqlParameter("@fechaHasta", fechaHasta.Value));
				}
				else if (fechaDesde.HasValue) // Solo fecha desde
				{
					sql += " AND c.fechaInicio >= @fechaDesde ";
					parameters.Add(new MySqlParameter("@fechaDesde", fechaDesde.Value));
				}
				else if (fechaHasta.HasValue) // Solo fecha hasta
				{
					sql += " AND c.fechaFinAnt <= @fechaHasta ";
					parameters.Add(new MySqlParameter("@fechaHasta", fechaHasta.Value));
				}

				// Filtro por cantidad de días restantes hasta el fin
				// if (dias.HasValue)
				// {
				// 	sql += " AND DATEDIFF(c.fechaFin, CURDATE()) = @dias ";
				// 	parameters.Add(new MySqlParameter("@dias", dias.Value));
				// }
				if (dias.HasValue)
				{
					switch (dias.Value)
					{
						case 30:
							sql += " AND c.fechaFin BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 1 MONTH) ";
							break;
						case 60:
							sql += " AND c.fechaFin BETWEEN DATE_ADD(CURDATE(), INTERVAL 1 MONTH) AND DATE_ADD(CURDATE(), INTERVAL 2 MONTH) ";
							break;
						case 90:
							sql += " AND c.fechaFin BETWEEN DATE_ADD(CURDATE(), INTERVAL 2 MONTH) AND DATE_ADD(CURDATE(), INTERVAL 3 MONTH) ";
							break;
						default:
							// Si se pasa otro valor, se puede ignorar o lanzar excepción
							break;
					}
				}

				// Paginado
				sql += $" ORDER BY c.id LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";

				using (var command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddRange(parameters.ToArray());
					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							Contrato contrato = new Contrato
							{
								Id = reader.GetInt32(nameof(Contrato.Id)),
								FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
								FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
								FechaFinAnt = reader.GetDateTime(nameof(Contrato.FechaFinAnt)),
								Estado = reader.GetString(nameof(Contrato.Estado)),
								Precio = reader.GetInt32(nameof(Contrato.Precio)),
								InquilinoId = reader.GetInt32(nameof(Contrato.InquilinoId)),
								InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId)),
								Inquilino = new Inquilino
								{
									Id = reader.GetInt32(nameof(Inquilino.Id)),
									Nombre = reader.GetString(nameof(Inquilino.Nombre)),
									Apellido = reader.GetString(nameof(Inquilino.Apellido)),
								},
								Inmueble = new Inmueble
								{
									Id = reader.GetInt32(nameof(Inmueble.Id)),
									Direccion = reader.GetString(nameof(Inmueble.Direccion)),
								},

							};
							contratos.Add(contrato);
						}
					}
					connection.Close();
				}

				return contratos;
			}
		}

		public int Alta(Contrato contrato)
		{
			int res = -1;
			// string fechaInicio = contrato.FechaInicio.ToString("yyyy-MM-dd");
			// string fechaFin = contrato.FechaFin.ToString("yyyy-MM-dd");

			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Contratos (FechaInicio, FechaFin, Estado, Precio, InquilinoId, InmuebleId , fechaFinAnt, UsuarioIdAlta ) " +
					"VALUES (@fechaInicio, @fechaFin, @estado, @precio, @InquilinoId, @InmuebleId, @fechaFinAnt, @UsuarioAlta);" +
					"SELECT LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new MySqlCommand(sql, connection))
				{
					//command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue($"@{nameof(contrato.FechaInicio)}", contrato.FechaInicio);
					command.Parameters.AddWithValue($"@{nameof(contrato.FechaFin)}", contrato.FechaFin);
					command.Parameters.AddWithValue($"@{nameof(contrato.Estado)}", contrato.Estado);
					command.Parameters.AddWithValue($"@{nameof(contrato.Precio)}", contrato.Precio);
					command.Parameters.AddWithValue($"@{nameof(contrato.InquilinoId)}", contrato.InquilinoId);
					command.Parameters.AddWithValue($"@{nameof(contrato.InmuebleId)}", contrato.InmuebleId);
					command.Parameters.AddWithValue($"@{nameof(contrato.FechaFinAnt)}", contrato.FechaFinAnt);
					command.Parameters.AddWithValue($"@{nameof(contrato.UsuarioAlta)}", contrato.UsuarioAlta);


					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					contrato.Id = res;
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
				string sql = $"DELETE FROM Contratos WHERE Id = {id}";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					// command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int Modificacion(Contrato contrato)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "UPDATE Contratos SET " +
					//	"FechaInicio=@fechaInicio, FechaFin=@fechaFin, Estado=@estado, Precio=@precio, InquilinoId=@inquilinoId, InmuebleId=@inmuebleId , FechaFinAnt=@fechaFinAnt " +
					"Estado=@estado, Precio=@precio, FechaFinAnt=@fechaFinAnt , UsuarioBaja=@usuarioBaja " +
					"WHERE Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@fechaInicio", contrato.FechaInicio);
					command.Parameters.AddWithValue("@fechaFin", contrato.FechaFin);
					command.Parameters.AddWithValue("@estado", contrato.Estado);
					command.Parameters.AddWithValue("@precio", contrato.Precio);
					command.Parameters.AddWithValue("@inquilinoId", contrato.InquilinoId);
					command.Parameters.AddWithValue("@inmuebleId", contrato.InmuebleId);
					command.Parameters.AddWithValue($"@{nameof(contrato.FechaFinAnt)}", contrato.FechaFinAnt);
					command.Parameters.AddWithValue($"@{nameof(contrato.UsuarioBaja)}", contrato.UsuarioBaja);
					command.Parameters.AddWithValue("@Id", contrato.Id);				

					connection.Open();	
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}


		public Contrato ObtenerPorId(int id)
		{
			Contrato? contrato = null;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $"SELECT c.Id, FechaInicio, FechaFin, c.Estado, c.Precio, InquilinoId, InmuebleId," +
							$" inq.Nombre, inq.Apellido, inm.Id, inm.Direccion, c.UsuarioAlta, c.UsuarioBaja " +
							$" FROM Contratos c INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id " +
							$"INNER JOIN Inmuebles inm ON inm.Id= c.InmuebleId " +
							"WHERE c.Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					// command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					// command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue($"@{nameof(id)}", id);
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						contrato = new Contrato
						{
							Id = reader.GetInt32(nameof(Contrato.Id)),
							FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
							FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
							Estado = reader.GetString(nameof(Contrato.Estado)),
							Precio = reader.GetInt32(nameof(Contrato.Precio)),
							InquilinoId = reader.GetInt32(nameof(Contrato.InquilinoId)),
							InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId)),
							Inquilino = new Inquilino
							{
								Id = reader.GetInt32(nameof(Inquilino.Id)),
								Nombre = reader.GetString(nameof(Inquilino.Nombre)),
								Apellido = reader.GetString(nameof(Inquilino.Apellido)),
							},
							Inmueble = new Inmueble
							{
								Id = reader.GetInt32(nameof(Inmueble.Id)),
								Direccion = reader.GetString(nameof(Inmueble.Direccion)),
							},
							UsuarioAlta = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.UsuarioAlta)))
								? (int?)null
								: reader.GetInt32(reader.GetOrdinal(nameof(Contrato.UsuarioAlta))),
							UsuarioBaja = reader.IsDBNull(reader.GetOrdinal(nameof(Contrato.UsuarioBaja)))
								? (int?)null
								: reader.GetInt32(reader.GetOrdinal(nameof(Contrato.UsuarioBaja))),
						};
					}
					connection.Close();
				}
			}
			return contrato;
		}


		public IList<Contrato> ObtenerAllContratosDeInmueble(int id)
		{
			IList<Contrato> res = new List<Contrato>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "SELECT c.Id, FechaInicio, FechaFin, c.Estado, c.Precio, InquilinoId, InmuebleId, c.fechaFinAnt, " +
					" inq.Nombre, inq.Apellido, inm.Direccion " +
					" FROM Contratos c INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id " +
					"INNER JOIN Inmuebles inm ON inm.Id= c.InmuebleId " +
					" WHERE inm.Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					//command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue($"@{nameof(id)}", id);
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato contrato = new Contrato
						{
							Id = reader.GetInt32(nameof(Contrato.Id)),
							FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
							FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
							Estado = reader.GetString(nameof(Contrato.Estado)),
							Precio = reader.GetInt32(nameof(Contrato.Precio)),
							InquilinoId = reader.GetInt32(nameof(Contrato.InquilinoId)),
							InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId)),
							FechaFinAnt = reader.GetDateTime(nameof(Contrato.FechaFinAnt)),
							Inquilino = new Inquilino
							{
								Id = reader.GetInt32(nameof(Inquilino.Id)),
								Nombre = reader.GetString(nameof(Inquilino.Nombre)),
								Apellido = reader.GetString(nameof(Inquilino.Apellido)),
							},
							Inmueble = new Inmueble
							{
								Id = reader.GetInt32(nameof(Inmueble.Id)),
								Direccion = reader.GetString(nameof(Inmueble.Direccion)),
							},

						};
						res.Add(contrato);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Contrato> ObtenerTodosVigentes()
		{
			IList<Contrato> res = new List<Contrato>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "SELECT c.Id, FechaInicio, FechaFin, c.Estado, c.Precio, InquilinoId, InmuebleId, c.fechaFinAnt " +
					" inq.Nombre, inq.Apellido, inm.Id, inm.Direccion " +
					" FROM Contratos c INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id " +
					"INNER JOIN Inmuebles inm ON inm.Id= c.InmuebleId " +
					"WHERE c.estado = 'Vigente' AND FechaFin > NOW() AND FechaInicio <= NOW()" + " ORDER BY FechaFin ASC";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					//command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato contrato = new Contrato
						{
							Id = reader.GetInt32(nameof(Contrato.Id)),
							FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
							FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
							Estado = reader.GetString(nameof(Contrato.Estado)),
							Precio = reader.GetInt32(nameof(Contrato.Precio)),
							InquilinoId = reader.GetInt32(nameof(Contrato.InquilinoId)),
							InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId)),
							FechaFinAnt = reader.GetDateTime(nameof(Contrato.FechaFinAnt)),
							Inquilino = new Inquilino
							{
								Id = reader.GetInt32(nameof(Inquilino.Id)),
								Nombre = reader.GetString(nameof(Inquilino.Nombre)),
								Apellido = reader.GetString(nameof(Inquilino.Apellido)),
							},
							Inmueble = new Inmueble
							{
								Id = reader.GetInt32(nameof(Inmueble.Id)),
								Direccion = reader.GetString(nameof(Inmueble.Direccion)),
							},

						};
						res.Add(contrato);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Contrato> ObtenerTodosNoVigentes()
		{
			IList<Contrato> res = new List<Contrato>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "SELECT c.Id, FechaInicio, FechaFin, c.Estado, c.Precio, InquilinoId, InmuebleId,c.FechaFinAnt " +
					" inq.Nombre, inq.Apellido, inm.Id, inm.Direccion  " +
					" FROM Contratos c INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id " +
					"INNER JOIN Inmuebles inm ON inm.Id= c.InmuebleId " +
					"WHERE  FechaInicio > NOW() OR FechaFin <= NOW() OR c.Estado='No vigente' " + " ORDER BY FechaFin ASC";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato contrato = new Contrato
						{
							Id = reader.GetInt32(0),
							FechaInicio = reader.GetDateTime(1),
							FechaFin = reader.GetDateTime(2),
							Estado = reader.GetString(3),
							Precio = reader.GetInt32(4),
							InquilinoId = reader.GetInt32(5),
							InmuebleId = reader.GetInt32(6),
							FechaFinAnt = reader.GetDateTime(nameof(Contrato.FechaFinAnt)),
							Inquilino = new Inquilino
							{
								Id = reader.GetInt32(5),
								Nombre = reader.GetString(7),
								Apellido = reader.GetString(8),
							},
							Inmueble = new Inmueble
							{
								Id = reader.GetInt32(6),
								Direccion = reader.GetString(9),
							},

						};
						res.Add(contrato);
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
					FROM Contratos
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
		
	}
}