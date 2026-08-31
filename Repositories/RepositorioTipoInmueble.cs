using mvc.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace mvc.Repositories
{
    public interface IRepositorioTipoInmueble
    {
        List<TipoInmueble> ObtenerTodos();
        IEnumerable<TipoInmueble> ObtenerPaginado(int pagina, int cantidadPorPagina, string? busqueda = null);
        TipoInmueble? ObtenerPorId(int id);
        void Alta(TipoInmueble tipo);
        void Modificacion(TipoInmueble tipo);
        void Baja(int id);
    }

    public class RepositorioTipoInmueble : IRepositorioTipoInmueble
    {
        private readonly string connectionString;

        public RepositorioTipoInmueble(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión DefaultConnection.");
        }

        public List<TipoInmueble> ObtenerTodos()
        {
            var tipos = new List<TipoInmueble>();

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT id, descripcion FROM tipo_inmueble ORDER BY descripcion ASC;";

            using var command = new MySqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                tipos.Add(new TipoInmueble
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Descripcion = reader["descripcion"].ToString() ?? ""
                });
            }

            return tipos;
        }

        public IEnumerable<TipoInmueble> ObtenerPaginado(int pagina, int cantidadPorPagina, string? busqueda = null)
        {
            var tipos = new List<TipoInmueble>();
            int offset = (pagina - 1) * cantidadPorPagina;

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT id, descripcion 
                FROM tipo_inmueble 
                WHERE @Busqueda IS NULL OR descripcion LIKE CONCAT('%', @Busqueda, '%')
                ORDER BY id DESC
                LIMIT @Cantidad OFFSET @Offset;
            ";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Offset", offset);
            command.Parameters.AddWithValue("@Cantidad", cantidadPorPagina);
            command.Parameters.AddWithValue("@Busqueda", busqueda ?? (object)DBNull.Value);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                tipos.Add(new TipoInmueble
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Descripcion = reader["descripcion"].ToString() ?? ""
                });
            }

            return tipos;
        }

        public TipoInmueble? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "SELECT id, descripcion FROM tipo_inmueble WHERE id = @id;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new TipoInmueble
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Descripcion = reader["descripcion"].ToString() ?? ""
                };
            }

            return null;
        }

        public void Alta(TipoInmueble tipo)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "INSERT INTO tipo_inmueble (descripcion) VALUES (@descripcion);";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@descripcion", tipo.Descripcion);

            command.ExecuteNonQuery();
        }

        public void Modificacion(TipoInmueble tipo)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "UPDATE tipo_inmueble SET descripcion = @descripcion WHERE id = @id;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@descripcion", tipo.Descripcion);
            command.Parameters.AddWithValue("@id", tipo.Id);

            command.ExecuteNonQuery();
        }

        public void Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "DELETE FROM tipo_inmueble WHERE id = @id;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }
    }
} 
