using mvc.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace mvc.Repositories
{
    public interface IRepositorioReserva
    {
        List<Reserva> ObtenerTodos();
        IEnumerable<Reserva> ObtenerPaginado(int pagina, int cantidadPorPagina, string? busqueda = null);
        Reserva? ObtenerPorId(int id);
        void Alta(Reserva reserva);
        void Modificacion(Reserva reserva);
        void Baja(int id);
        bool ValidarDisponibilidad(int idInmueble, DateTime fechaDesde, DateTime fechaHasta, int idReservaIgnorar = 0);
    }

    public class RepositorioReserva : IRepositorioReserva
    {
        private readonly string connectionString;

        public RepositorioReserva(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión DefaultConnection.");
        }

        public List<Reserva> ObtenerTodos()
        {
            var reservas = new List<Reserva>();
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT 
                    r.id, r.id_inmueble, r.id_inquilino, r.fecha_desde, r.fecha_hasta, 
                    r.monto_diario, r.activo, r.creado_por_user_id, r.terminado_por_user_id,
                    i.direccion AS inmueble_direccion,
                    CONCAT(inq.nombre, ' ', inq.apellido) AS inquilino_nombre
                FROM reserva r
                INNER JOIN inmueble i ON r.id_inmueble = i.id
                INNER JOIN inquilino inq ON r.id_inquilino = inq.id
                ORDER BY r.id DESC;
            ";

            using var command = new MySqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                reservas.Add(MapearReserva(reader));
            }

            return reservas;
        }

        public IEnumerable<Reserva> ObtenerPaginado(int pagina, int cantidadPorPagina, string? busqueda = null)
        {
            var reservas = new List<Reserva>();
            int offset = (pagina - 1) * cantidadPorPagina;

            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT 
                    r.id, r.id_inmueble, r.id_inquilino, r.fecha_desde, r.fecha_hasta, 
                    r.monto_diario, r.activo, r.creado_por_user_id, r.terminado_por_user_id,
                    i.direccion AS inmueble_direccion,
                    CONCAT(inq.nombre, ' ', inq.apellido) AS inquilino_nombre
                FROM reserva r
                INNER JOIN inmueble i ON r.id_inmueble = i.id
                INNER JOIN inquilino inq ON r.id_inquilino = inq.id
                WHERE @Busqueda IS NULL 
                   OR i.direccion LIKE CONCAT('%', @Busqueda, '%') 
                   OR inq.nombre LIKE CONCAT('%', @Busqueda, '%')
                   OR inq.apellido LIKE CONCAT('%', @Busqueda, '%')
                ORDER BY r.id DESC
                LIMIT @Cantidad OFFSET @Offset;
            ";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Offset", offset);
            command.Parameters.AddWithValue("@Cantidad", cantidadPorPagina);
            command.Parameters.AddWithValue("@Busqueda", busqueda ?? (object)DBNull.Value);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                reservas.Add(MapearReserva(reader));
            }

            return reservas;
        }

        public Reserva? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT 
                    r.id, r.id_inmueble, r.id_inquilino, r.fecha_desde, r.fecha_hasta, 
                    r.monto_diario, r.activo, r.creado_por_user_id, r.terminado_por_user_id,
                    i.direccion AS inmueble_direccion,
                    CONCAT(inq.nombre, ' ', inq.apellido) AS inquilino_nombre
                FROM reserva r
                INNER JOIN inmueble i ON r.id_inmueble = i.id
                INNER JOIN inquilino inq ON r.id_inquilino = inq.id
                WHERE r.id = @id;
            ";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapearReserva(reader);
            }

            return null;
        }

        public void Alta(Reserva reserva)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                INSERT INTO reserva (
                    id_inmueble, id_inquilino, fecha_desde, fecha_hasta, 
                    monto_diario, activo, creado_por_user_id
                ) VALUES (
                    @id_inmueble, @id_inquilino, @fecha_desde, @fecha_hasta, 
                    @monto_diario, @activo, @creado_por_user_id
                );
            ";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id_inmueble", reserva.IdInmueble);
            command.Parameters.AddWithValue("@id_inquilino", reserva.IdInquilino);
            command.Parameters.AddWithValue("@fecha_desde", reserva.FechaDesde);
            command.Parameters.AddWithValue("@fecha_hasta", reserva.FechaHasta);
            command.Parameters.AddWithValue("@monto_diario", reserva.MontoDiario);
            command.Parameters.AddWithValue("@activo", reserva.Activo);
            command.Parameters.AddWithValue("@creado_por_user_id", reserva.CreadoPorUserId);

            command.ExecuteNonQuery();
        }

        public void Modificacion(Reserva reserva)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                UPDATE reserva SET 
                    id_inmueble = @id_inmueble, 
                    id_inquilino = @id_inquilino, 
                    fecha_desde = @fecha_desde, 
                    fecha_hasta = @fecha_hasta, 
                    monto_diario = @monto_diario, 
                    activo = @activo
                WHERE id = @id;
            ";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", reserva.Id);
            command.Parameters.AddWithValue("@id_inmueble", reserva.IdInmueble);
            command.Parameters.AddWithValue("@id_inquilino", reserva.IdInquilino);
            command.Parameters.AddWithValue("@fecha_desde", reserva.FechaDesde);
            command.Parameters.AddWithValue("@fecha_hasta", reserva.FechaHasta);
            command.Parameters.AddWithValue("@monto_diario", reserva.MontoDiario);
            command.Parameters.AddWithValue("@activo", reserva.Activo);

            command.ExecuteNonQuery();
        }

        public void Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = "UPDATE reserva SET activo = 0 WHERE id = @id;";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public bool ValidarDisponibilidad(int idInmueble, DateTime fechaDesde, DateTime fechaHasta, int idReservaIgnorar = 0)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT COUNT(*) FROM reserva 
                WHERE id_inmueble = @idInmueble 
                AND activo = 1 
                AND id != @idReservaIgnorar
                AND (fecha_desde <= @fechaHasta AND fecha_hasta >= @fechaDesde);
            ";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idInmueble", idInmueble);
            command.Parameters.AddWithValue("@fechaDesde", fechaDesde);
            command.Parameters.AddWithValue("@fechaHasta", fechaHasta);
            command.Parameters.AddWithValue("@idReservaIgnorar", idReservaIgnorar);

            int cantidad = Convert.ToInt32(command.ExecuteScalar());
            return cantidad == 0;
        }

        private Reserva MapearReserva(MySqlDataReader reader)
        {
            return new Reserva
            {
                Id = Convert.ToInt32(reader["id"]),
                IdInmueble = Convert.ToInt32(reader["id_inmueble"]),
                IdInquilino = Convert.ToInt32(reader["id_inquilino"]),
                FechaDesde = Convert.ToDateTime(reader["fecha_desde"]),
                FechaHasta = Convert.ToDateTime(reader["fecha_hasta"]),
                MontoDiario = Convert.ToDecimal(reader["monto_diario"]),
                Activo = Convert.ToInt32(reader["activo"]),
                CreadoPorUserId = Convert.ToInt32(reader["creado_por_user_id"]),
                TerminadoPorUserId = reader["terminado_por_user_id"] == DBNull.Value ? null : Convert.ToInt32(reader["terminado_por_user_id"]),
                InmuebleDireccion = reader["inmueble_direccion"].ToString(),
                InquilinoNombre = reader["inquilino_nombre"].ToString()
            };
        }
    }
}  
