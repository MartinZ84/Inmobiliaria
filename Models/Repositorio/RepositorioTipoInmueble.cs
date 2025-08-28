using System.Data;
using MySql.Data.MySqlClient;
using Inmobiliaria.Models.Entidades;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria.Models.Repositorio
{
    public class RepositorioTipoInmueble
    {
        private readonly string connectionString;

        public RepositorioTipoInmueble(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IList<TipoInmueble> ObtenerTodos()
        {
            var tiposInmueble = new List<TipoInmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    SELECT id, descripcion 
                    FROM tiposInmuebles 
                    ORDER BY Descripcion";

                using (var command = new MySqlCommand(sql, connection))
                {
                    // command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var i = new TipoInmueble
                        {
                            Id = reader.GetInt32(nameof(TipoInmueble.Id)),
                            Descripcion = reader.GetString(nameof(TipoInmueble.Descripcion))
                        };
                        tiposInmueble.Add(i);
                    }
                }
            }
            return tiposInmueble;
        }

        public TipoInmueble? ObtenerPorId(int id)
        {
            TipoInmueble? tipoInmueble = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    SELECT id, descripcion 
                    FROM tiposInmuebles 
                    WHERE id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        tipoInmueble = new TipoInmueble
                        {
                            Id = reader.GetInt32(nameof(TipoInmueble.Id)),
                            Descripcion = reader.GetString(nameof(TipoInmueble.Descripcion))
                        };
                    }
                }
            }
            return tipoInmueble;
        }

        public int Alta(TipoInmueble tipoInmueble)
        {
            int id = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    INSERT INTO tiposInmuebles (descripcion) 
                    VALUES (@descripcion);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@descripcion", tipoInmueble.Descripcion);
                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return id;
        }

        public int Modificacion(TipoInmueble tipoInmueble)
        {
            int filasAfectadas = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    UPDATE tiposInmuebles 
                    SET descripcion = @descripcion
                    WHERE id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@descripcion", tipoInmueble.Descripcion);
                    command.Parameters.AddWithValue("@id", tipoInmueble.Id);
                    connection.Open();
                    filasAfectadas = command.ExecuteNonQuery();
                }
            }
            return filasAfectadas;
        }


        public int Baja(int id)
        {
            int filasAfectadas = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"
                    DELETE FROM tiposInmuebles
                    WHERE id = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    filasAfectadas = command.ExecuteNonQuery();
                }
            }
            return filasAfectadas;
        }

        public SelectList GetTipos()
        {
            var tiposInmueble = ObtenerTodos();
            return new SelectList(tiposInmueble, "Id", "Descripcion");
        }
    }
}