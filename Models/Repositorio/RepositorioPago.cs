using MySql.Data.MySqlClient;
using Inmobiliaria.Models.Entidades;
using System.Security.Claims;

namespace Inmobiliaria.Models.Repositorio
{

	public class RepositorioPago : RepositorioBase
	{
		public RepositorioPago(IConfiguration configuration) : base(configuration)
		{

		}



		public IList<Pago> ObtenerTodos()
		{
			IList<Pago> res = new List<Pago>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "SELECT p.Id, NroPago, FechaPago, Estado, Importe, ContratoId, Concepto, p.Estado" +
					" " +
					" FROM Pagos p INNER JOIN Contratos c ON p.ContratoId = c.Id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					//command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Pago pago = new Pago
						{
							Id = reader.GetInt32(nameof(Pago.Id)),
							NroPago = reader.GetInt32(nameof(Pago.NroPago)),
							FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
							Importe = reader.GetDecimal(nameof(Pago.Importe)),
							ContratoId = reader.GetInt32(nameof(Pago.ContratoId)),
							Concepto = reader.GetString(nameof(Pago.Concepto))
						};
						res.Add(pago);
					}
					connection.Close();
				}
			}
			return res;
		}
		public int Alta(Pago pago)
		{
			int res = -1;

			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Pagos (NroPago, FechaPago, Importe, ContratoId, Concepto, Estado, UsuarioIdAlta) " +
					"VALUES (@nroPago, @fechaPago, @importe, @contratoId, @Concepto, @Estado, @usuarioIdAlta); " +
					"SELECT LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new MySqlCommand(sql, connection))
				{
					//command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue($"@{nameof(pago.NroPago)}", pago.NroPago);
					command.Parameters.AddWithValue($"@{nameof(pago.FechaPago)}", pago.FechaPago);
					command.Parameters.AddWithValue($"@{nameof(pago.Importe)}", pago.Importe);
					command.Parameters.AddWithValue($"@{nameof(pago.ContratoId)}", pago.ContratoId);
					command.Parameters.AddWithValue($"@{nameof(pago.Concepto)}", pago.Concepto);
					command.Parameters.AddWithValue($"@{nameof(pago.Estado)}", pago.Estado);
					command.Parameters.AddWithValue($"@{nameof(pago.usuarioIdAlta)}", pago.usuarioIdAlta);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					pago.Id = res;
					connection.Close();
				}
			}
			return res;
		}

		public int Baja(int id, int usuarioBaja)
		{
			int res = -1;
			
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				//string sql = $"DELETE FROM Pagos WHERE Id = {id}";
				string sql = $"UPDATE Pagos SET Estado='Anulado', UsuarioIdBaja = @usuarioBaja WHERE Id = {id}";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					// command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@usuarioBaja", usuarioBaja);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int Modificacion(Pago pago)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "UPDATE Pagos SET " +
					// "NroPago=@nroPago, FechaPago=@fechaPago, Importe=@importe, ContratoId=@contratoId " +
					" FechaPago=@fechaPago, Importe=@importe, Concepto=@concepto, Estado=@estado " +
					"WHERE Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@fechaPago", pago.FechaPago);
					command.Parameters.AddWithValue("@importe", pago.Importe);
					command.Parameters.AddWithValue("@estado", pago.Estado);
					command.Parameters.AddWithValue("@concepto", pago.Concepto);
					command.Parameters.AddWithValue("@id", pago.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}


		public Pago ObtenerPorId(int id)
		{
			Pago? pago = null;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "SELECT p.Id, NroPago, FechaPago,  Importe, ContratoId , concepto, p.Estado , p.UsuarioIdAlta, p.UsuarioIdBaja " +
					" FROM Pagos p INNER JOIN Contratos c ON p.ContratoId = c.Id " +
							"WHERE p.Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					// command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					// command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue($"@{nameof(id)}", id);
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						pago = new Pago
						{
							Id = reader.GetInt32(nameof(Pago.Id)),
							NroPago = reader.GetInt32(nameof(Pago.NroPago)),
							FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
							Importe = reader.GetDecimal(nameof(Pago.Importe)),
							ContratoId = reader.GetInt32(nameof(Pago.ContratoId)),
							Concepto = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Concepto))) ? "" : reader.GetString(nameof(Pago.Concepto)),
							Estado = reader.GetString(nameof(Pago.Estado)),
							usuarioIdAlta = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.usuarioIdAlta)))
										? (int?)null
										: reader.GetInt32(reader.GetOrdinal(nameof(Pago.usuarioIdAlta))),
							usuarioIdBaja = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.usuarioIdBaja)))
										? (int?)null
										: reader.GetInt32(reader.GetOrdinal(nameof(Pago.usuarioIdBaja))),
						};
					}
					connection.Close();
				}
			}
			return pago;
		}

		public IList<Pago> ObtenerPagosPorContrato(int id)
		{
			IList<Pago> res = new List<Pago>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT p.Id, NroPago, FechaPago, Importe, ContratoId, Concepto, p.Estado, p.UsuarioIdAlta, p.UsuarioIdBaja
                       FROM Pagos p 
                       INNER JOIN Contratos c ON p.ContratoId = c.Id
                       WHERE c.Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue($"@{nameof(id)}", id);
					connection.Open();
					var reader = command.ExecuteReader();

					while (reader.Read())
					{
						Pago pago = new Pago
						{
							Id = reader.GetInt32(nameof(Pago.Id)),
							NroPago = reader.GetInt32(nameof(Pago.NroPago)),
							FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
							Importe = reader.GetDecimal(nameof(Pago.Importe)),
							ContratoId = reader.GetInt32(nameof(Pago.ContratoId)),
							Concepto = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Concepto)))
										? ""
										: reader.GetString(nameof(Pago.Concepto)),
							Estado = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.Estado)))
										? ""
										: reader.GetString(nameof(Pago.Estado)),
							usuarioIdAlta = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.usuarioIdAlta)))
										? (int?)null
										: reader.GetInt32(reader.GetOrdinal(nameof(Pago.usuarioIdAlta))),
							usuarioIdBaja = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.usuarioIdBaja)))
										? (int?)null
										: reader.GetInt32(reader.GetOrdinal(nameof(Pago.usuarioIdBaja))),
						};
						res.Add(pago);
					}
				}
			}
			return res;
		}


		public int ObtenerCantidadPagos(int id)
		{
			int nroPago = 0;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "SELECT COUNT(nroPago) FROM pagos WHERE contratoId=@id";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					// command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					// command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue($"@{nameof(id)}", id);
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						nroPago = reader.GetInt32(0);
					}
					connection.Close();
				}
			}
			return nroPago + 1;
		}

		public int ObtenerCantidadPagosAbonados(int id)
		{
			int nroPago = 0;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "SELECT COUNT(nroPago) FROM pagos WHERE contratoId=@id AND estado='Abonado'";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					// command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					// command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue($"@{nameof(id)}", 	id);
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						nroPago = reader.GetInt32(0);
					}
					connection.Close();
				}
			}
			return nroPago ;
		}

	}



}