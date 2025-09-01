using System.Data;
using MySql.Data.MySqlClient;
using Inmobiliaria.Models.Entidades;
using Inmobiliaria.Models.Enums;

namespace Inmobiliaria.Models.Repositorio
{
    public class RepositorioInmueble
    {
        private readonly string connectionString;

        public RepositorioInmueble(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IList<Inmueble> ObtenerTodos()
        {
            var inmuebles = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    SELECT i.*, p.nombre as PropietarioNombre, p.apellido as PropietarioApellido, ti.descripcion as TipoInmuebleDescripcion
                    FROM inmuebles i 
                    INNER JOIN propietarios p ON i.propietarioId = p.id
                    INNER JOIN tiposInmuebles ti ON i.tipInmId = ti.id
                    ORDER BY i.id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(MapearInmueble(reader));
                        }
                    }
                }
            }
            return inmuebles;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? inmueble = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    SELECT i.*, p.nombre as PropietarioNombre, p.apellido as PropietarioApellido,
                           p.dni as PropietarioDni, p.telefono as PropietarioTelefono, p.email as PropietarioEmail,
                           ti.descripcion as TipoInmuebleDescripcion
                    FROM inmuebles i 
                    INNER JOIN propietarios p ON i.propietarioId = p.id
                    INNER JOIN tiposInmuebles ti ON i.tipInmId = ti.id
                    WHERE i.id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = MapearInmuebleConPropietario(reader);
                        }
                    }
                }
            }
            return inmueble;
        }

        public int Alta(Inmueble inmueble)
        {
            int id = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    INSERT INTO inmuebles (direccion, ambientes, superficie, tipInmId, uso, precio, latitud, longitud, estado, propietarioId, imagenes)
                    VALUES (@direccion, @ambientes, @superficie, @tipoInmId, @uso, @precio, @latitud, @longitud, @estado, @propietarioId, @imagenes);
                    SELECT LAST_INSERT_ID()";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@superficie", inmueble.Superficie ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@tipoInmId", inmueble.TipoInmId);
                    command.Parameters.AddWithValue("@uso", inmueble.Uso);
                    command.Parameters.AddWithValue("@precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@latitud", inmueble.Latitud ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", inmueble.Longitud ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@estado", inmueble.Estado);
                    command.Parameters.AddWithValue("@propietarioId", inmueble.PropietarioId);
                    command.Parameters.AddWithValue("@imagenes", inmueble.Imagenes ?? (object)DBNull.Value);

                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return id;
        }

        public int Modificacion(Inmueble inmueble)
        {
            int result = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    UPDATE inmuebles SET 
                        direccion = @direccion, 
                        ambientes = @ambientes, 
                        superficie = @superficie, 
                        tipInmId = @tipoInmId, 
                        uso = @uso, 
                        precio = @precio, 
                        latitud = @latitud, 
                        longitud = @longitud, 
                        estado = @estado, 
                        propietarioId = @propietarioId,
                        imagenes = @imagenes
                    WHERE id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", inmueble.Id);
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@superficie", inmueble.Superficie ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@tipoInmId", inmueble.TipoInmId);
                    command.Parameters.AddWithValue("@uso", inmueble.Uso);
                    command.Parameters.AddWithValue("@precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@latitud", inmueble.Latitud ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", inmueble.Longitud ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@estado", inmueble.Estado);
                    command.Parameters.AddWithValue("@propietarioId", inmueble.PropietarioId);
                    command.Parameters.AddWithValue("@imagenes", inmueble.Imagenes ?? (object)DBNull.Value);

                    connection.Open();
                    result = command.ExecuteNonQuery();
                }
            }
            return result;
        }

        // public int Baja(int id)
        // {
        //     int result = 0;
        //     using (var connection = new MySqlConnection(connectionString))
        //     {
        //         // Verificar si el inmueble tiene contratos asociados
        //         var sqlCheck = "SELECT COUNT(*) FROM contratos WHERE inmuebleId = @id";
        //         using (var commandCheck = new MySqlCommand(sqlCheck, connection))
        //         {
        //             commandCheck.Parameters.AddWithValue("@id", id);
        //             connection.Open();
        //             int contractCount = Convert.ToInt32(commandCheck.ExecuteScalar());

        //             if (contractCount > 0)
        //             {
        //                 throw new Exception("No se puede eliminar el inmueble porque tiene contratos asociados.");
        //             }
        //         }

        //         var sql = "DELETE FROM inmuebles WHERE id = @id";
        //         using (var command = new MySqlCommand(sql, connection))
        //         {
        //             command.Parameters.AddWithValue("@id", id);
        //             result = command.ExecuteNonQuery();
        //         }
        //     }
        //     return result;
        // }
        public int BajaLogica(int id)
        {
            int result = 0;
            var baja = (int)EstadoInmueble.Baja;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var sql = "UPDATE inmuebles SET estado = @baja WHERE id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@baja", baja);
                    result = command.ExecuteNonQuery();
                }
            }
            return result;
        }

        public IList<Inmueble> BuscarInmueblesConValidacion(int? tipo = null,
            string? uso = null, int? estado = null, int? precioMin = null, int? precioMax = null)
        {
            var inmuebles = new List<Inmueble>();

            // Validar que al menos un criterio esté presente
            if (!tipo.HasValue && string.IsNullOrWhiteSpace(uso) && !estado.HasValue &&
                !precioMin.HasValue && !precioMax.HasValue)
            {
                return inmuebles; // Retornar lista vacía si no hay criterios
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    SELECT i.*, p.nombre as PropietarioNombre, p.apellido as PropietarioApellido, ti.descripcion as TipoInmuebleDescripcion
                    FROM inmuebles i 
                    INNER JOIN propietarios p ON i.propietarioId = p.id
                    INNER JOIN tiposInmuebles ti ON i.tipInmId = ti.id
                    WHERE 1=1 ";

                var parameters = new List<MySqlParameter>();

                // if (!string.IsNullOrWhiteSpace(direccion))
                // {
                //     sql += " AND i.direccion LIKE @direccion";
                //     parameters.Add(new MySqlParameter("@direccion", $"%{direccion.Trim()}%"));
                // }

                if (tipo.HasValue)
                {
                    sql += " AND i.tipInmId = @tipo";
                    parameters.Add(new MySqlParameter("@tipo", tipo.Value));
                }

                if (!string.IsNullOrWhiteSpace(uso))
                {
                    sql += " AND i.uso LIKE @uso";
                    parameters.Add(new MySqlParameter("@uso", $"%{uso.Trim()}%"));
                }

                if (estado.HasValue)
                {
                    sql += " AND i.estado = @estado";
                    parameters.Add(new MySqlParameter("@estado", estado.Value));
                }
                else
                {
                    // Si no se especifica estado, excluir los dados de baja
                    sql += " AND i.estado <> @estadoBaja";
                    parameters.Add(new MySqlParameter("@estadoBaja", (int)EstadoInmueble.Baja));
                }

                if (precioMin.HasValue)
                {
                    sql += " AND i.precio >= @precioMin";
                    parameters.Add(new MySqlParameter("@precioMin", precioMin.Value));
                }

                if (precioMax.HasValue)
                {
                    sql += " AND i.precio <= @precioMax";
                    parameters.Add(new MySqlParameter("@precioMax", precioMax.Value));
                }

                sql += " ORDER BY i.id LIMIT 200"; // Limitar a 200 resultados

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(MapearInmueble(reader));
                        }
                    }
                }
            }
            return inmuebles;
        }

        public IList<Inmueble> ObtenerDisponibles()
        {
            var inmuebles = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    SELECT i.*, p.nombre as PropietarioNombre, p.apellido as PropietarioApellido, ti.descripcion as TipoInmuebleDescripcion
                    FROM inmuebles i 
                    INNER JOIN propietarios p ON i.propietarioId = p.id
                    INNER JOIN tiposInmuebles ti ON i.tipInmId = ti.id
                    WHERE i.estado = 1
                    ORDER BY i.id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(MapearInmueble(reader));
                        }
                    }
                }
            }
            return inmuebles;
        }

        private Inmueble MapearInmueble(MySqlDataReader reader)
        {
            return new Inmueble
            {
                Id = reader.GetInt32("id"),
                Direccion = reader.GetString("direccion"),
                Ambientes = reader.GetInt32("ambientes"),
                Superficie = reader.IsDBNull("superficie") ? null : reader.GetDecimal("superficie"),
                TipoInmId = reader.GetInt32("tipInmId"),
                Uso = reader.GetString("uso"),
                Precio = reader.GetInt32("precio"),
                Latitud = reader.IsDBNull("latitud") ? null : reader.GetDecimal("latitud"),
                Longitud = reader.IsDBNull("longitud") ? null : reader.GetDecimal("longitud"),
                EstadoBd = reader.GetInt32("estado"),
                PropietarioId = reader.GetInt32("propietarioId"),
                Imagenes = reader.IsDBNull("imagenes") ? null : reader.GetString("imagenes"),
                Propietario = new Propietario
                {
                    Id = reader.GetInt32("propietarioId"),
                    Nombre = reader.GetString("PropietarioNombre"),
                    Apellido = reader.GetString("PropietarioApellido")
                },
                TipoInmueble = new TipoInmueble
                {
                    Id = reader.GetInt32("tipInmId"),
                    Descripcion = reader.GetString("TipoInmuebleDescripcion")
                }

            };
        }

        private Inmueble MapearInmuebleConPropietario(MySqlDataReader reader)
        {
            return new Inmueble
            {
                Id = reader.GetInt32("id"),
                Direccion = reader.GetString("direccion"),
                Ambientes = reader.GetInt32("ambientes"),
                Superficie = reader.IsDBNull("superficie") ? null : reader.GetDecimal("superficie"),
                TipoInmId = reader.GetInt32("tipInmId"),
                Uso = reader.GetString("uso"),
                Precio = reader.GetInt32("precio"),
                Latitud = reader.IsDBNull("latitud") ? null : reader.GetDecimal("latitud"),
                Longitud = reader.IsDBNull("longitud") ? null : reader.GetDecimal("longitud"),
                EstadoBd = reader.GetInt32("estado"),
                PropietarioId = reader.GetInt32("propietarioId"),
                Imagenes = reader.IsDBNull("imagenes") ? null : reader.GetString("imagenes"),
                Propietario = new Propietario
                {
                    Id = reader.GetInt32("propietarioId"),
                    Nombre = reader.GetString("PropietarioNombre"),
                    Apellido = reader.GetString("PropietarioApellido"),
                    Dni = reader.GetString("PropietarioDni"),
                    Telefono = reader.GetString("PropietarioTelefono"),
                    Email = reader.GetString("PropietarioEmail")
                },
                TipoInmueble = new TipoInmueble
                {
                    Id = reader.GetInt32("tipInmId"),
                    Descripcion = reader.GetString("TipoInmuebleDescripcion")
                }

            };
        }

        public int ObtenerCantidad()
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
					SELECT COUNT(Id)
					FROM Inmuebles
                    WHERE Estado <> 3;
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



        public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 5)
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @$"
					SELECT i.*, p.nombre as PropietarioNombre, p.apellido as PropietarioApellido, ti.descripcion as TipoInmuebleDescripcion
                    FROM inmuebles i 
                    INNER JOIN propietarios p ON i.propietarioId = p.id
                    INNER JOIN tiposInmuebles ti ON i.tipInmId = ti.id
                    WHERE i.estado <> 3
                    ORDER BY i.id
					LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}        
				";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res.Add(MapearInmueble(reader));
                        }
                    }
                }
            }
            return res;
        }

        public IList<Inmueble> ObtenerTodosDisponibles()
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT i.Id, Direccion, Ambientes, Superficie, Tipo, Uso, Precio,Latitud, Longitud, Estado, PropietarioId," +
                    " p.Nombre, p.Apellido" +
                    " FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.Id " +
                    " WHERE i.Estado = 'Disponible'";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    //command.CommandType = CommandType.Text;

                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        res.Add(MapearInmueble(reader));
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int BuscarDisponibilidad(int InmuebleId, DateTime FechaInicio, DateTime FechaFin)
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(CONTRATOS.inmuebleId) " +
                         "FROM contratos WHERE " +
                          "CONTRATOS.inmuebleId=@inmuebleId " + " AND " +
                         "(( contratos.fechaInicio  between @FechaInicio and @FechaFin) " +
                              " OR (contratos.fechaFin  between @FechaInicio and @FechaFin)) ";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    //command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue($"@{nameof(InmuebleId)}", InmuebleId);
                    command.Parameters.AddWithValue($"@{nameof(FechaInicio)}", FechaInicio);
                    command.Parameters.AddWithValue($"@{nameof(FechaFin)}", FechaFin);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = reader.GetInt32(0);
                    }
                    ;

                }
                connection.Close();

            }
            return res;


        }

    }
}



