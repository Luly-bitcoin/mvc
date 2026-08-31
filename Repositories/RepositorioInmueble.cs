using mvc.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace mvc.Repositories
{
    public interface IRepositorioInmueble
    {
        List<Inmueble> ObtenerTodos();
        IEnumerable<Inmueble> ObtenerPaginado(int pagina, int cantidadPorPagina, string? ubicacion = null, int? personas = null, int? idTipo = null);
        Inmueble? ObtenerPorId(int id);
        void Alta(Inmueble inmueble);
        void Modificacion(Inmueble inmueble);
        void Baja(int id);
    }

    public class RepositorioInmueble : IRepositorioInmueble
    {
        private readonly string connectionString;

        public RepositorioInmueble(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión DefaultConnection.");
        }

        public List<Inmueble> ObtenerTodos()
        {
            var inmuebles = new List<Inmueble>();

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT 
                    i.id,
                    i.id_propietario,
                    i.id_tipo_inmueble,
                    i.direccion,
                    i.cupo,
                    i.coord,
                    i.precio,
                    i.activo,
                    i.foto_portada,
                    i.fotos,
                    CONCAT(p.nombre, ' ', p.apellido) AS propietario_nombre,
                    t.descripcion AS tipo_nombre
                FROM inmueble i
                INNER JOIN propietario p ON i.id_propietario = p.id
                INNER JOIN tipo_inmueble t ON i.id_tipo_inmueble = t.id
                ORDER BY i.id DESC;
            ";

            using var command = new MySqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                inmuebles.Add(MapearInmueble(reader));
            }

            return inmuebles;
        }

        public IEnumerable<Inmueble> ObtenerPaginado(int pagina, int cantidadPorPagina, string? ubicacion = null, int? personas = null, int? idTipo = null)
        {
            var inmuebles = new List<Inmueble>();
            int offset = (pagina - 1) * cantidadPorPagina;

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT 
                    i.id,
                    i.id_propietario,
                    i.id_tipo_inmueble,
                    i.direccion,
                    i.cupo,
                    i.coord,
                    i.precio,
                    i.activo,
                    i.foto_portada,
                    i.fotos,
                    CONCAT(p.nombre, ' ', p.apellido) AS propietario_nombre,
                    t.descripcion AS tipo_nombre
                FROM inmueble i
                INNER JOIN propietario p ON i.id_propietario = p.id
                INNER JOIN tipo_inmueble t ON i.id_tipo_inmueble = t.id
                WHERE i.activo = 1
                AND (@Ubicacion IS NULL OR i.direccion LIKE CONCAT('%', @Ubicacion, '%'))
                AND (@Personas IS NULL OR i.cupo >= @Personas)
                AND (@IdTipo IS NULL OR i.id_tipo_inmueble = @IdTipo)
                ORDER BY i.id DESC
                LIMIT @Cantidad OFFSET @Offset;
            ";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Offset", offset);
            command.Parameters.AddWithValue("@Cantidad", cantidadPorPagina);
            command.Parameters.AddWithValue("@Ubicacion", ubicacion ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Personas", personas ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IdTipo", idTipo ?? (object)DBNull.Value);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                inmuebles.Add(MapearInmueble(reader));
            }

            return inmuebles;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT 
                    i.id,
                    i.id_propietario,
                    i.id_tipo_inmueble,
                    i.direccion,
                    i.cupo,
                    i.coord,
                    i.precio,
                    i.activo,
                    i.foto_portada,
                    i.fotos,
                    CONCAT(p.nombre, ' ', p.apellido) AS propietario_nombre,
                    t.descripcion AS tipo_nombre
                FROM inmueble i
                INNER JOIN propietario p ON i.id_propietario = p.id
                INNER JOIN tipo_inmueble t ON i.id_tipo_inmueble = t.id
                WHERE i.id = @id;
            ";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapearInmueble(reader);
            }

            return null;
        }

        public void Alta(Inmueble inmueble)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                INSERT INTO inmueble
                (
                    id_propietario,
                    id_tipo_inmueble,
                    direccion,
                    cupo,
                    coord,
                    precio,
                    activo,
                    foto_portada,
                    fotos
                )
                VALUES
                (
                    @id_propietario,
                    @id_tipo_inmueble,
                    @direccion,
                    @cupo,
                    @coord,
                    @precio,
                    @activo,
                    @foto_portada,
                    @fotos
                );
            ";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id_propietario", inmueble.IdPropietario);
            command.Parameters.AddWithValue("@id_tipo_inmueble", inmueble.IdTipoInmueble);
            command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
            command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
            command.Parameters.AddWithValue("@coord", (object?)inmueble.Coord ?? DBNull.Value);
            command.Parameters.AddWithValue("@precio", inmueble.Precio);
            command.Parameters.AddWithValue("@activo", inmueble.Activo);
            command.Parameters.AddWithValue("@foto_portada", (object?)inmueble.FotoPortada ?? DBNull.Value);
            command.Parameters.AddWithValue("@fotos", (object?)inmueble.Fotos ?? DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void Modificacion(Inmueble inmueble)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                UPDATE inmueble
                SET
                    id_propietario = @id_propietario,
                    id_tipo_inmueble = @id_tipo_inmueble,
                    direccion = @direccion,
                    cupo = @cupo,
                    coord = @coord,
                    precio = @precio,
                    activo = @activo,
                    foto_portada = @foto_portada,
                    fotos = @fotos
                WHERE id = @id;
            ";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", inmueble.Id);
            command.Parameters.AddWithValue("@id_propietario", inmueble.IdPropietario);
            command.Parameters.AddWithValue("@id_tipo_inmueble", inmueble.IdTipoInmueble);
            command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
            command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
            command.Parameters.AddWithValue("@coord", (object?)inmueble.Coord ?? DBNull.Value);
            command.Parameters.AddWithValue("@precio", inmueble.Precio);
            command.Parameters.AddWithValue("@activo", inmueble.Activo);
            command.Parameters.AddWithValue("@foto_portada", (object?)inmueble.FotoPortada ?? DBNull.Value);
            command.Parameters.AddWithValue("@fotos", (object?)inmueble.Fotos ?? DBNull.Value);

            command.ExecuteNonQuery();
        }

        public void Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                DELETE FROM inmueble
                WHERE id = @id;
            ";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }

        private Inmueble MapearInmueble(MySqlDataReader reader)
        {
            return new Inmueble
            {
                Id = Convert.ToInt32(reader["id"]),
                IdPropietario = Convert.ToInt32(reader["id_propietario"]),
                IdTipoInmueble = Convert.ToInt32(reader["id_tipo_inmueble"]),
                Direccion = reader["direccion"].ToString() ?? "",
                Cupo = Convert.ToInt32(reader["cupo"]),
                Coord = reader["coord"] == DBNull.Value
                    ? null
                    : reader["coord"].ToString(),
                Precio = Convert.ToDecimal(reader["precio"]),
                Activo = Convert.ToInt32(reader["activo"]),
                FotoPortada = reader["foto_portada"] == DBNull.Value
                    ? null
                    : reader["foto_portada"].ToString(),
                Fotos = reader["fotos"] == DBNull.Value
                    ? null
                    : reader["fotos"].ToString(),
                PropietarioNombre = reader["propietario_nombre"].ToString(),
                TipoNombre = reader["tipo_nombre"].ToString()
            };
        }
    }
}